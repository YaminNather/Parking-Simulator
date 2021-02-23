using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISafetyCheckListener
{
    void fOnEnterSafetyRange();
    void fOnExitSafetyRange();
    
    void fOnEnterWarningRange();
    void fOnExitWarningRange();

    void fOnEnterDangerRange();
    void fOnExitDangerRange();
}
