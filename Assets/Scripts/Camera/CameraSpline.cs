using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpline : MonoBehaviour
{
    public BezierSpline MySpline;
    public SplineMovement AnimalSpline;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Animal.CurrentAnimal != null)
        {
            float firstDist = -1;
            int firstIndex = 0;
            for (int i = 0; i < AnimalSpline.points.Length; ++i)
            {
                float dist = Vector3.Distance(Animal.CurrentAnimal.transform.position, AnimalSpline.GetPosition(i));

                if (dist < firstDist || firstDist < 0)
                {
                    firstIndex = i;
                    firstDist = dist;
                }
            }

            float secondDist = -1;
            int secondIndex = firstIndex;
            for (int i = firstIndex == 0? 0 : firstIndex - 1; i < AnimalSpline.points.Length; ++i)
            {
                float dist = Vector3.Distance(Animal.CurrentAnimal.transform.position, AnimalSpline.GetPosition(i));

                if ((dist < secondDist || secondDist < 0) && firstIndex != i)
                {
                    secondDist = dist;
                    secondIndex = i;
                }
            }

            float t = firstDist / (firstDist + secondDist);
            float coreDist = Vector3.Distance(AnimalSpline.GetPosition(firstIndex), AnimalSpline.GetPosition(secondIndex));
            float ab = firstDist + secondDist;
            float mult = (coreDist / ab);
            t = t * mult;

            SplineMovement.Point first = AnimalSpline.points[firstIndex];
            SplineMovement.Point second = AnimalSpline.points[secondIndex];

            if (firstIndex > secondIndex)
            {
                t = 1 - t;
                first = AnimalSpline.points[secondIndex];
                second = AnimalSpline.points[firstIndex];
            }

            float splineT = Mathf.Lerp(first.time, second.time, t);
            transform.position = MySpline.GetPoint(splineT);
        }
    }
}