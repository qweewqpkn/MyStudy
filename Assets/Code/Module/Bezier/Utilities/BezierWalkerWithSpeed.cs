using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BezierSolution
{
    public class BezierWalkerWithSpeed : MonoBehaviour, IBezierWalker
    {
        public enum TravelMode { Once, Loop, PingPong };
        private Transform cachedTransform;
        public TravelMode travelMode;
        public float rotationLerpModifier = 10f;
        public bool lookForward = true;
        public List<float> mSpeedList;
        public float speed;
        public BezierSpline Spline { get; set; }

        private float progress = 0f;
        public float NormalizedT
        {
            get { return progress; }
            set { progress = value; }
        }

        private bool isGoingForward = true;
        public bool MovingForward { get { return (speed > 0f) == isGoingForward; } }

        public UnityEvent onPathCompleted = new UnityEvent();
        private bool onPathCompletedCalledAt1 = false;
        private bool onPathCompletedCalledAt0 = false;

        private void Awake()
        {
            cachedTransform = transform;
            mSpeedList = new List<float>() { 5, 10 };
        }

        public void Init(List<float> speedList)
        {
            if(speedList == null || speedList.Count == 0)
            {
                return;
            }

            mSpeedList = speedList;
        }

        private float GetSpeed(float progress)
        {
            int state = Spline.GetStage(progress);
            if(state >= 0 && state < mSpeedList.Count)
            {
                return mSpeedList[state];
            }
            
            return 0;
        }

		private void Update()
		{
            speed = GetSpeed(progress);
			float targetSpeed = ( isGoingForward ) ? speed : -speed;
            cachedTransform.position = Spline.MoveAlongSpline(ref progress, targetSpeed * Time.deltaTime);

			bool movingForward = MovingForward;
			if( lookForward )
			{
				Quaternion targetRotation;
				if( movingForward )
					targetRotation = Quaternion.LookRotation(Spline.GetTangent( progress ) );
				else
					targetRotation = Quaternion.LookRotation( -Spline.GetTangent( progress ) );

				cachedTransform.rotation = Quaternion.Lerp( cachedTransform.rotation, targetRotation, rotationLerpModifier * Time.deltaTime );
			}

			if( movingForward )
			{
				if( progress >= 1f )
				{
					if( !onPathCompletedCalledAt1 )
					{
						onPathCompleted.Invoke();
						onPathCompletedCalledAt1 = true;
					}

					if( travelMode == TravelMode.Once )
						progress = 1f;
					else if( travelMode == TravelMode.Loop )
						progress -= 1f;
					else
					{
						progress = 2f - progress;
						isGoingForward = !isGoingForward;
					}
				}
				else
				{
					onPathCompletedCalledAt1 = false;
				}
			}
			else
			{
				if( progress <= 0f )
				{
					if( !onPathCompletedCalledAt0 )
					{
						onPathCompleted.Invoke();
						onPathCompletedCalledAt0 = true;
					}

					if( travelMode == TravelMode.Once )
						progress = 0f;
					else if( travelMode == TravelMode.Loop )
						progress += 1f;
					else
					{
						progress = -progress;
						isGoingForward = !isGoingForward;
					}
				}
				else
				{
					onPathCompletedCalledAt0 = false;
				}
			}
		}
	}
}