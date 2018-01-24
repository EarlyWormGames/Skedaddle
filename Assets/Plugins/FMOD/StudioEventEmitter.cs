﻿using UnityEngine;
using System;
using System.Collections.Generic;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/FMOD Studio Event Emitter")]
    public class StudioEventEmitter : MonoBehaviour
    {
        [EventRef]
        public String Event;
        public EmitterGameEvent PlayEvent;
        public EmitterGameEvent StopEvent;
        public String CollisionTag;
        public bool AllowFadeout = true;
        public bool TriggerOnce = false;

        public ParamRef[] Params;

        [Range(0f, 1f)]
        public float PlayVolume = 1f;
        public eAudioGroup Group;
        
        private FMOD.Studio.EventDescription eventDescription;
        private FMOD.Studio.EventInstance instance;
        private bool hasTriggered;
        private bool isQuitting;

        private float m_Volume = 1f;

        private bool m_bStopped = false;

        void Start() 
        {
            AudioGroup.AddToGroup(Group, this);
            RuntimeUtils.EnforceLibraryOrder();
            HandleGameEvent(EmitterGameEvent.Created);
        }

        void OnApplicationQuit()
        {
            isQuitting = true;
        }

        void OnDestroy()
        {
            if (!isQuitting)
            {
                TriggerCue();
                HandleGameEvent(EmitterGameEvent.Destroy);
                if (instance != null && instance.isValid())
                {
                    RuntimeManager.DetachInstanceFromGameObject(instance);
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (String.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
            {
                HandleGameEvent(EmitterGameEvent.TriggerEnter);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (String.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
            {
                HandleGameEvent(EmitterGameEvent.TriggerExit);
            }
        }

        void OnCollisionEnter()
        {
            HandleGameEvent(EmitterGameEvent.CollisionEnter);
        }

        void OnCollisionExit()
        {
            HandleGameEvent(EmitterGameEvent.CollisionExit);
        }

        void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (PlayEvent == gameEvent)
            {
                Play();
            }
            if (StopEvent == gameEvent)
            {
                Stop();
            }
        }

        void Lookup()
        {
            eventDescription = RuntimeManager.GetEventDescription(Event);
        }

        public void Play()
        {
            m_bStopped = false;

            if (TriggerOnce && hasTriggered)
            {
                return;
            }

            if (String.IsNullOrEmpty(Event))
            {
                return;
            }

            if (eventDescription == null)
            {
                Lookup();
            }

            bool isOneshot = false;
            if (!Event.StartsWith("snapshot", StringComparison.CurrentCultureIgnoreCase))
            {
                eventDescription.isOneshot(out isOneshot);
            }
            bool is3D;
            eventDescription.is3D(out is3D);

            if (instance != null && !instance.isValid())
            {
                instance = null;
            }

            // Let previous oneshot instances play out
            if (isOneshot && instance != null)
            {
                instance.release();
                instance = null;
            }

            if (instance == null)
            {
                eventDescription.createInstance(out instance);

                // Only want to update if we need to set 3D attributes
                if (is3D)
                {
                    var rigidBody = GetComponent<Rigidbody>();
                    var transform = GetComponent<Transform>();
                    instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody));
                    RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody);
                }
            }

            if (Params != null)
            {
                foreach (var param in Params)
                {
                    instance.setParameterValue(param.Name, param.Value);
                }
            }

            instance.start();

            instance.getVolume(out m_Volume);
            m_Volume = PlayVolume * 10f;
            instance.setVolume(m_Volume);

            hasTriggered = true;

        }

        public void Stop()
        {
            if (instance != null)
            {
                m_bStopped = true;
                instance.stop(AllowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
                instance.release();
                instance = null;
            }
        }

        public bool Stopped()
        {
            return m_bStopped;
        }
        
        public void SetParameter(string name, float value)
        {
            if (instance != null)
            {
                instance.setParameterValue(name, value);
            }
        }

        public bool IsPlaying()
        {
            if (instance != null && instance.isValid())
            {
                FMOD.Studio.PLAYBACK_STATE playbackState;
                instance.getPlaybackState(out playbackState);
                bool playing = playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED;
                return playing;
            }
            return false;
        }

        public void TriggerCue()
        {
            if (instance != null)
            {
                instance.triggerCue();
            }
        }

        public bool isPaused
        {
            get
            {
                bool value = false;
                if (instance != null)
                    instance.getPaused(out value);
                return value;
            }
            set
            {
                if (instance != null)
                    instance.setPaused(value);
            }
        }

        public float volume
        {
            get
            {
                if (instance == null)
                    return 0f;

                return m_Volume;
            }
            set
            {
                if (instance != null)
                    m_Volume = value;
            }
        }

        void Update()
        {
            if (instance != null)
            {
                instance.setVolume(m_Volume + AudioGroup.GetVolume(Group));
            }
        }
    }
}
