// 씬 종류
public enum eSceneType
{
    None,
    Entrance,
    Patch,
    Login,
    Loading,
    InGame,
}

// 국가
public enum eNationType
{
    Korea,
}

// 서비스 모드
public enum eServiceMode
{
    None,
    Live,           // Live
    Review,         // 리뷰제출용
    QA,             // QA용
    Dev,            // 개발용
}

// 서비스 상태
public enum eServiceState
{
    None,
    Run,            // 서비스 중
    Check,          // 점검 중
    ConnectMarket,  // 마켓연결
}

// 번들 패킹 타입
public enum eBundlePackType
{
    None,           // 아무것도 안함
    All,            // 전체 번들 리패킹
    Update,         // 변경된 리소스가 포함되는 번들만 패킹
}

// 데이터 종류
public enum eDataType
{
    None,
    LocalTable,
    ServerTable,
    Resources,
    Scene,
    AssetsBundle,
}

// 테이블 포맷타입
public enum eTableType
{
    None,
    SQLite,
    Json,
    XML,
    Byte,
}

// 리소스 데이터 종류
public enum eResourceType
{
    None,
    Prefab,
    Animation,
    Texture,
    Sound,
    Material,
    Text,
}

// 오브젝트 제거타입
public enum eObjectDestoryType
{
    Never,          // 제거 안함
    ChangeScene,    // 씬이 바뀔 때
}

// Bool
public enum eBOOL
{
    None   = -1,
    False  = 0,
    True   = 1,
}

// 방향
public enum eDirection
{
    None,
    Front,
    Back,
    Left,
    Right,
    Top,
    Bottom,
}

// 순서
public enum eOrderNum
{
    None,
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    Sixth,
    Seventh,
    Eighth,
    Ninth,
    Tenth,
}

// 판정
public enum eDecision
{
    Miss,
    Bad,
    Normal,
    Good,
}