using UnityEngine;
using System.Collections;

public static class SHPhysics
{
    public static Vector3 m_vGravity = new Vector3(0.0f, -9.81f, 0.0f);

    // Spring 알고리즘(Runge-Kutta Method) : 중심점을 기준으로 포인트위치가 스프링처럼 움직인다.
    // 단, 속도는 누적 갱신되는 값이므로 호출하는 곳에서 멤버변수로 가지고 있어야한다.
    public static Vector3 CalculationSpring(Vector3 vCenter, Vector3 vPoint, ref Vector3 vSpeed, float fSpringPower, float fSpringDamping)
    {
        //Runge-Kutta Method
        Vector3 vK = fSpringPower * (vPoint - vCenter); // 스프링상수
        Vector3 vB = vSpeed * fSpringDamping;           // 감쇄폭
        Vector3 vF = -vK - vB;
        return CalculationEuler(vF, vPoint, ref vSpeed);
    }

    // 오일러해법( 운동방정식, 질량포함 )
    public static Vector3 CalculationEuler(Vector3 vForce, Vector3 vPoint, ref Vector3 vSpeed, float fMass)
    {
        return CalculationEuler(vForce * fMass, vPoint, ref vSpeed);
    }

    // 오일러해법( 운동방정식 )
    public static Vector3 CalculationEuler(Vector3 vForce, Vector3 vPoint, ref Vector3 vSpeed)
    {
        // 속도 = 가속도 * 시간
        vSpeed += (vForce * Single.Timer.m_fFixedDeltaTime);

        // 위치 = 속도 * 시간
        vPoint += (vSpeed * Single.Timer.m_fFixedDeltaTime);

        return vPoint;
    }

    // 유도 알고리즘
    public static void GuidedMissile(ref Vector3 vPos, ref Vector3 vDirection, Vector3 vTarget, float fHomingAngle, float fSpeed, bool bDirect = false)
    {
        // 예외처리 : 꺽이는 각도가 0이면 방향이 zero가되어 큰 문제가 생긴다.
        if (0.0f == fHomingAngle)
            fHomingAngle = 5.0f;

        // 요마이 작으면 임의축이 잘 안뽑히넹...
        if (0.1 > vTarget.magnitude)
            vTarget.z = -0.1f;

        // 타켓으로 향하는 방향벡터
        Vector3 vTargetDist = (vTarget - vPos).normalized;

        // 예외처리 : 이미 타켓에 도착했는가?
        if (Vector3.zero == vTargetDist)
            return;

        // 회전 전 타켓과 진행 방향간 X/Y 부호기록
        bool bMarkX1 = (0.0f < vTargetDist.x - vDirection.x);
        bool bMarkY1 = (0.0f < vTargetDist.y - vDirection.y);

        if (true == bDirect)
        {
            // 다이렉트 회전
            vDirection = vTargetDist;
        }
        else
        {
            // 임의축으로 회전
            Vector3 vAxis = Vector3.Cross(vDirection, vTargetDist).normalized;
            Quaternion qRotate = Quaternion.AngleAxis(fHomingAngle, vAxis);
            vDirection = (qRotate * vDirection).normalized;
        }

        bool bMarkX2 = (0.0f < vTargetDist.x - vDirection.x);
        bool bMarkY2 = (0.0f < vTargetDist.y - vDirection.y);

        // 방향 부호가 변했다면 타켓방향보다 회전이 더 크게 된것으로 타켓방향으로 처리 안해주면 지그재그로 흔들림
        if (bMarkX1 != bMarkX2)
            vDirection.x = vTargetDist.x;
        if (bMarkY1 != bMarkY2)
            vDirection.y = vTargetDist.y;

        // 회전된 방향으로 진행
        Vector3 vSpeed = (vDirection * fSpeed);
        vPos = CalculationEuler(Vector3.zero, vPos, ref vSpeed);
    }
}
