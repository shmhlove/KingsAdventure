using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SHFirebase : SHSingleton<SHFirebase>
{
    private SHFirebaseAuth m_pAuth = new SHFirebaseAuth();
    public SHFirebaseAuth Auth { get { return m_pAuth; } }

    public override void OnInitialize()
    {
        SetDontDestroy();

        Auth.OnInitialize();
    }

    public override void OnFinalize()
    {
        Auth.OnFinalize();
    }
}
