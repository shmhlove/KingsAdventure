public enum eErrorCode
{
    // 공통
    Succeed,
    Failed,

    // 씬 관련
    Scene_Load_Fail,

    // 리소스 관련
    Resource_Load_Fail,
    Resource_Not_ExsitTable,

    // 테이블 관련
    Table_Load_Fail,
    Table_Not_AddClass,
    Table_Not_Override,
    Table_Not_ExsitFile,
    Table_Error_Grammar,

    // 업데이트 관련
    Patch_Table,
    Patch_Bundle_Download_Fail,
}