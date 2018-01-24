public class MusicHandler : Singleton<MusicHandler>
{
    [System.Serializable]
    public class MusicTrack
    {
        public string name = "";
        public AreaMusic track;
    }

    public enum TrackType
    {
        MUSIC,
        AMBIENCE,
    }

    public MusicTrack[] m_MusicTracks;
    public MusicTrack[] m_AmbienceTracks;

    public static void PlayTrack(string a_sound, TrackType a_type)
    {
        if (Instance.m_MusicTracks == null)
            return;

        if (a_type == TrackType.MUSIC)
        {
            foreach (MusicTrack track in Instance.m_MusicTracks)
            {
                if (track.name.ToLower() == a_sound.ToLower())
                {
                    track.track.Play();
                    return;
                }
            }
        }
        else if (a_type == TrackType.AMBIENCE)
        {
            foreach (MusicTrack track in Instance.m_AmbienceTracks)
            {
                if (track.name.ToLower() == a_sound.ToLower())
                {
                    track.track.Play();
                    return;
                }
            }
        }
    }

    public static void StopTrack(string a_sound, TrackType a_type)
    {
        if (Instance.m_MusicTracks == null)
            return;

        if (EWApplication.Instance.m_bRefresh)
            return;

        if (a_type == TrackType.MUSIC)
        {
            foreach (MusicTrack track in Instance.m_MusicTracks)
            {
                if (track.name.ToLower() == a_sound.ToLower())
                {
                    track.track.Stop();
                    return;
                }
            }
        }
        else if (a_type == TrackType.AMBIENCE)
        {
            foreach (MusicTrack track in Instance.m_AmbienceTracks)
            {
                if (track.name.ToLower() == a_sound.ToLower())
                {
                    track.track.TriggerCue();
                    return;
                }
            }
        }
    }

    public static void SetIndex(int a_index)
    {
        if (Instance.m_MusicTracks != null)
        {
            foreach (MusicTrack track in Instance.m_MusicTracks)
            {
                track.track.SetIndex(a_index);
            }
        }
    }

    public static void SetParameter(string a_param, float a_val)
    {
        if (Instance.m_MusicTracks != null)
        {
            foreach (MusicTrack track in Instance.m_MusicTracks)
            {
                track.track.SetParam(a_param, a_val);
            }
        }
    }
}
