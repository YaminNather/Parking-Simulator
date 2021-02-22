using UnityEngine;

public class SafetyCheckListener : MonoBehaviour
{
    internal void fRecieveRayFromSafetyChecker(Ranges range)
    {
        mRecievedRay = true;
        fSwitchRange(range);
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
        if (mCurRange != Ranges.Safety && mRecievedRay == false)
        {
            fSwitchRange(Ranges.Safety);
        }

        mRecievedRay = false;
    }

    protected virtual void fOnEnterSafetyRange() {}

    protected virtual void fOnExitSafetyRange() {}

    protected virtual void fOnEnterWarningRange() {}

    protected virtual void fOnExitWarningRange() {}

    protected virtual void fOnEnterDangerRange() {}

    protected virtual void fOnExitDangerRange() {}


    private bool mRecievedRay = false;
    internal Ranges mCurRange = Ranges.Safety;
}

public enum Ranges { Safety, Warning, Danger }