using UnityEngine;

public enum WJObjectRootType
{
    None = 0
}

public enum WJObjectType
{
    None = 0,
    WJ2DMap,
    WJ2DPlayer,
    WJEnemySpawner,
    WJ2DBullitSpawner
}

public enum WJ2DGameStat
{
    None = 0,
    Start,
    Pause,
    Over,
    Roby
}

public static class WJ2DGameManagerExtension
{
    public static string Get2DWJPath(this DaniTechGameManager gameManager, 
        WJObjectType objType, 
        WJObjectRootType objRootType = WJObjectRootType.None)
    {
        string path = string.Empty; // "" == string.Empty

        if(objRootType == WJObjectRootType.None)
        {
            path = $"Prefabs/2D/{objType}";
            return path;
        }
        // 신규UI추가 2) Resources.Load를 할 경로를 직접 명시한다
        // 해당 경로는 프로젝트창에서 Resources/Prefabs/UI폴더 내에 있는 RootType 폴더명과 UIType 프리팹 이름과 동일해야 한다! (ex. ContentUI/DNMyProfilePopup)
        path = $"Prefabs/UI/{objRootType}/{objType}";
        return path;
    }

    public static void StartGame(this DaniTechGameManager gameManager)
    {
        GameObject liveRoot = new GameObject("LiveRoot");
        liveRoot.transform.SetParent(gameManager.ReturnGameRootTransfrom());
        liveRoot.transform.localPosition = Vector3.zero;
        liveRoot.transform.localRotation = Quaternion.identity;
        liveRoot.transform.localScale = Vector3.one;
        gameManager.SetLiveRoot(liveRoot.transform);
        // 게임 시작 시 만들 것들을 할당
        gameManager.InitCurTime(65f);
        // 위에는 테스트용도
        gameManager.MakeMap();
        gameManager.MakePlayer();
        gameManager.MakeEnemySpawner();
        gameManager.MakeBulitSpawner();
        gameManager.InitCatchEnemyCount();
        gameManager.SetGameStat(WJ2DGameStat.Start);
    }

}
