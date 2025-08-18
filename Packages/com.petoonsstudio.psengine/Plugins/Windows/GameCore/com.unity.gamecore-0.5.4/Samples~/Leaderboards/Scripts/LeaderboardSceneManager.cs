using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.GameCore;
using UnityEngine.UI;
using System.Threading;

namespace LeaderboardSample
{
    public class LeaderboardSceneManager : MonoBehaviour
    {
        public class RowUIData
        {
            public XUserHandle userObject;
            public GameObject gameObject;
            public Transform objTransform;
            public Text Column1;
            public Text Column2;
            public Text Column3;
            public bool done;
        }

        public class RowData
        {
            public uint Rank;
            public string GamerTag;
            public string ColumnValues;
            public string StatName;
            public string StatType;
            public string StatValue;
        }

        public enum RowType
        {
            Leaderboard,
            Stat
        }

        public GameObject LeaderboardDash;
        public List<GameObject> LeaderboardUIDashGO = new List<GameObject>();
        public ScrollRect LeaderboardUIScrollRect;

        public List<RowUIData> RowUIDataList = new List<RowUIData>();
        public Text AboutSceneText;

        public Button StatsMazeButton;
        public Button StatsCaveButton;
        public Button LeaderboardMazeButton;
        public Button LeaderboardCaveButton;
        public Button PlayGameButton;

        public List<RowData> RowDataList = new List<RowData>();

        public void Start()
        {
            AboutSceneText.text = k_AboutSceneText;

            GamingRuntimeManager.Instance.UserManager.UsersChanged += UserManager_UsersChanged;
#if UNITY_GAMECORE
            UnityEngine.GameCore.GameCorePLM.OnSuspendingEvent += GameCorePLM_OnSuspendingEvent;
#endif //UNITY_GAMECORE

            LeaderboardMazeButton.interactable = false;
            LeaderboardCaveButton.interactable = false;
            StatsMazeButton.interactable = false;
            StatsCaveButton.interactable = false;
            PlayGameButton.interactable = false;
        }

        private void GameCorePLM_OnSuspendingEvent()
        {
#if UNITY_GAMECORE
            UnityEngine.GameCore.GameCorePLM.AmReadyToSuspendNow();
#endif //UNITY_GAMECORE
        }

        public void Update()
        {
            if (m_LeaderboardRowsAvailable.WaitOne(0))
            {
                RowUIData newRowData = CreateRowDash();
                RowUIDataList.Add(newRowData);
                switch (m_RowType)
                {
                    case RowType.Leaderboard:
                        FormatLeaderboardHeader(newRowData);
                        FormatLeaderboardRows();
                        break;
                    case RowType.Stat:
                        FormatStatHeader(newRowData);
                        FormatStatRows();
                        break;
                }


                LeaderboardUIScrollRect.verticalNormalizedPosition = 0;

                m_RequestInProgress = false;
            }
        }

        public void AddUserWithUI()
        {
            // We attempt to add the first user as the default one, the others need to be explicitely selected
            if (GamingRuntimeManager.Instance.UserManager.UserDataList.Count == 0)
                GamingRuntimeManager.Instance.UserManager.AddDefaultUserSilently(AddUserCompleted);
            else
                GamingRuntimeManager.Instance.UserManager.AddUserWithUI(AddUserCompleted);
        }

        public void PlayGame()
        {
            var result = ExploreGameSimulation.PlayGame();
            var json = result.ToEventJson();

            Debug.Log("Sending event json: " + json);

            UserManager.UserData currentUserData = GamingRuntimeManager.Instance.UserManager.UserDataList[0];
            SDK.XBL.XblEventsWriteInGameEvent(currentUserData.m_context, "AreaExplored", json, "{}");
        }

        public void GetMazeLeaderboard()
        {
            if (m_RequestInProgress)
                return;

            m_RequestInProgress = true;
            m_RowType = RowType.Leaderboard;
            ClearLeaderboardData();

            UserManager.UserData currentUserData = GamingRuntimeManager.Instance.UserManager.UserDataList[0];
            leaderboardContext = new LeaderboardsContext(currentUserData.userXUID, currentUserData.m_context);
            leaderboardContext.QueryLeaderboards("MostTraveledMaze", "AreaExplored.Environment.Maze", XblSocialGroupType.None, null, LeaderboardDataAvailable);
        }

        public void GetCaveLeaderboard()
        {
            if (m_RequestInProgress)
                return;

            m_RequestInProgress = true;
            m_RowType = RowType.Leaderboard;
            ClearLeaderboardData();

            UserManager.UserData currentUserData = GamingRuntimeManager.Instance.UserManager.UserDataList[0];
            leaderboardContext = new LeaderboardsContext(currentUserData.userXUID, currentUserData.m_context);
            leaderboardContext.QueryLeaderboards("MostTraveledCave", "AreaExplored.Environment.Cave", XblSocialGroupType.None, null, LeaderboardDataAvailable);
        }

        public void GetMazeStats()
        {
            if (m_RequestInProgress)
                return;

            m_RequestInProgress = true;
            m_RowType = RowType.Stat;
            ClearLeaderboardData();

            UserManager.UserData currentUserData = GamingRuntimeManager.Instance.UserManager.UserDataList[0];
            leaderboardContext = new LeaderboardsContext(currentUserData.userXUID, currentUserData.m_context);
            leaderboardContext.QueryStatistics("AreaExplored.Environment.Maze", StatDataAvailable);
        }

        public void GetCaveStats()
        {
            if (m_RequestInProgress)
                return;

            m_RequestInProgress = true;
            m_RowType = RowType.Stat;
            ClearLeaderboardData();

            UserManager.UserData currentUserData = GamingRuntimeManager.Instance.UserManager.UserDataList[0];
            leaderboardContext = new LeaderboardsContext(currentUserData.userXUID, currentUserData.m_context);
            leaderboardContext.QueryStatistics("AreaExplored.Environment.Cave", StatDataAvailable);
        }

        const string k_AboutSceneText = "This demo shows how to use leaderboards and stats.\nSelect the Login User button to get started.";
        const string k_NowUseButtonsText = "Now use the Play Game button to simulate a game and generate some stats.\nUse either the Stats or Leaderboards to get and display data rows below.";

        private Semaphore m_LeaderboardRowsAvailable = new Semaphore(0, 1);
        private RowType m_RowType;
        private bool m_RequestInProgress = false;
        private string ColumnNames;
        private LeaderboardsContext leaderboardContext;

        private void UserManager_UsersChanged(object sender, XUserChangeEvent e)
        {
        }

        private void FormatLeaderboardHeader(RowUIData uiData)
        {
            // all other columns by default have the correct text in them for the header
            uiData.Column3.text = ColumnNames;
        }

        private void FormatLeaderboardRows()
        {
            int numRows = RowDataList.Count;
            for (int i = 0; i < numRows; ++i)
            {
                RowUIData newRowData = CreateRowDash();
                UpdateLeaderboardRowDash(newRowData, RowDataList[i]);

                RowUIDataList.Add(newRowData);
            }
        }

        private void FormatStatHeader(RowUIData uiData)
        {
            uiData.Column1.text = "Stat Name";
            uiData.Column2.text = "Stat Type";
            uiData.Column3.text = "Value";
        }

        private void FormatStatRows()
        {
            int numRows = RowDataList.Count;
            for (int i = 0; i < numRows; ++i)
            {
                RowUIData newRowData = CreateRowDash();
                UpdateStatRowDash(newRowData, RowDataList[i]);

                RowUIDataList.Add(newRowData);
            }
        }

        private void ClearLeaderboardData()
        {
            foreach (var row in RowUIDataList)
            {
                Destroy(row.gameObject);
            }
            LeaderboardUIDashGO.Clear();
            RowUIDataList.Clear();
            RowDataList.Clear();
        }

        public void LeaderboardDataAvailable(int hresult, uint pageNumber, XblLeaderboardResult xblLeaderboardResult)
        {
            ColumnNames = "";

            foreach (var column in xblLeaderboardResult.Columns)
            {
                ColumnNames += "| " + column.StatName + " |";
            }

            foreach (var row in xblLeaderboardResult.Rows)
            {
                RowData rowData = new RowData();
                rowData.Rank = row.Rank;
                rowData.GamerTag = row.Gamertag;
                rowData.ColumnValues = "";
                foreach (var column in row.ColumnValues)
                {
                    rowData.ColumnValues += "| " + column + " |";
                }

                if (row.Rank > RowDataList.Count)
                    RowDataList.Add(rowData);
                else
                    RowDataList.Insert((int)row.Rank, rowData);
            }

            if (!xblLeaderboardResult.HasNext)
            {
                m_LeaderboardRowsAvailable.Release(1);
            }
        }

        public void StatDataAvailable(int hresult, XblUserStatisticsResult xblStatResult)
        {
            ColumnNames = "Value";

            foreach (var serviceConfigStat in xblStatResult.ServiceConfigStatistics)
            {
                foreach (var stat in serviceConfigStat.Statistics)
                {
                    RowData rowData = new RowData();
                    rowData.StatName = stat.StatisticName;
                    rowData.StatType = stat.StatisticType;
                    rowData.StatValue = stat.Value;

                    RowDataList.Add(rowData);
                }
            }

            m_LeaderboardRowsAvailable.Release(1);
        }

        private void AddUserCompleted(UserManager.UserOpResult result)
        {
            switch (result)
            {
                case UserManager.UserOpResult.Success:
                    {
                        LeaderboardMazeButton.interactable = true;
                        LeaderboardCaveButton.interactable = true;
                        StatsMazeButton.interactable = true;
                        StatsCaveButton.interactable = true;
                        PlayGameButton.interactable = true;

                        AboutSceneText.text = k_NowUseButtonsText;
                        break;
                    }
                case UserManager.UserOpResult.NoDefaultUser:
                    {
                        GamingRuntimeManager.Instance.UserManager.AddUserWithUI(AddUserCompleted);
                        break;
                    }
                case UserManager.UserOpResult.UnknownError:
                    {
                        Debug.Log("Error adding user.");
                        break;
                    }
                default:
                    break;
            }
        }

        private RowUIData CreateRowDash()
        {
            RowUIData rowUiData = new RowUIData();

            GameObject tempObject = Instantiate(LeaderboardDash);
            LeaderboardUIDashGO.Add(tempObject);
            tempObject.transform.SetParent(LeaderboardUIScrollRect.content.gameObject.transform, false);
            tempObject.SetActive(true);

            rowUiData.gameObject = tempObject;

            rowUiData.objTransform = rowUiData.gameObject.transform;
            rowUiData.Column1 = rowUiData.gameObject.GetComponent<RowValues>().Column1;
            rowUiData.Column2 = rowUiData.gameObject.GetComponent<RowValues>().Column2;
            rowUiData.Column3 = rowUiData.gameObject.GetComponent<RowValues>().Column3;

            return rowUiData;
        }

        private void UpdateLeaderboardRowDash(RowUIData currentRowInformation, RowData currentRowData)
        {
            currentRowInformation.Column1.text = currentRowData.Rank.ToString();
            currentRowInformation.Column2.text = currentRowData.GamerTag;
            currentRowInformation.Column3.text = currentRowData.ColumnValues;
        }

        private void UpdateStatRowDash(RowUIData currentRowInformation, RowData currentRowData)
        {
            currentRowInformation.Column1.text = currentRowData.StatName;
            currentRowInformation.Column2.text = currentRowData.StatType;
            currentRowInformation.Column3.text = currentRowData.StatValue;
        }
    }
}
