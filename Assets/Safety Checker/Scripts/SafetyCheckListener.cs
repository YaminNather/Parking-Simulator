using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace SafetyChecker
{
    public class SafetyCheckListener : MonoBehaviour
    {
        private void Start()
        {
            mRecievedRanges = new List<Ranges>();
        }

        internal void fRecieveRayFromSafetyChecker(Ranges range)
        {
            mRecievedRay = true;
            mRecievedRanges.Add(range);
            //fSwitchRange(range);
        }

        private void fSwitchRange(Ranges range)
        {
            if (range == mCurRange)
                return;

            switch (mCurRange)
            {
                case Ranges.Safety:
                    fOnExitSafetyRange();
                    break;

                case Ranges.Warning:
                    fOnExitWarningRange();
                    break;

                case Ranges.Danger:
                    fOnExitDangerRange();
                    break;
            }


            switch (range)
            {
                case Ranges.Safety:
                    fOnEnterSafetyRange();
                    break;

                case Ranges.Warning:
                    fOnEnterWarningRange();
                    break;

                case Ranges.Danger:
                    fOnEnterDangerRange();
                    break;
            }

            mCurRange = range;
        }

        protected virtual void LateUpdate()
        {
            if (mRecievedRay == false)
            {
                if (mCurRange != Ranges.Safety) 
                    fSwitchRange(Ranges.Safety);
            }
            else
            {
                Ranges selectedRange = (mRecievedRanges.Contains(Ranges.Danger)) ? Ranges.Danger : Ranges.Warning;
                fSwitchRange(selectedRange);
            }

            mRecievedRay = false;
            mRecievedRanges.Clear();
        }

        protected virtual void fOnEnterSafetyRange()
        {
        }

        protected virtual void fOnExitSafetyRange()
        {
        }

        protected virtual void fOnEnterWarningRange()
        {
        }

        protected virtual void fOnExitWarningRange()
        {
        }

        protected virtual void fOnEnterDangerRange()
        {
        }

        protected virtual void fOnExitDangerRange()
        {
        }


        #region Variables
        private bool mRecievedRay = false;
        private List<Ranges> mRecievedRanges;
        internal Ranges mCurRange = Ranges.Safety;
        #endregion
    }

    public enum Ranges
    {
        Safety,
        Warning,
        Danger
    }
}