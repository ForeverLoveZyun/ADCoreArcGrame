﻿using ArcSoftFace.ADCoreSystem.ADcoreModel;
using ArcSoftFace.ADCoreSystem;
using ArcSoftFace.GameNet;
using System.Collections.Generic;
using System;
using System.Windows.Forms;

namespace ArcSoftFace.GameCommon
{
    public  class NewGroupSql
    {
        public static NewGroupSql Instance;
        LocalNetServer netServer = new LocalNetServer();
        public void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Req_NewGroupGetGroup(GameMsg msg)
        {
            GameMsg msgs = new GameMsg()
            {
                cmd = CMD.Rsp_NewGroupGetGroup,
            };
            IsExitenceUserExcelData(GameConst.DBUserExcel);
            try
            {
                String path = Application.StartupPath + GameConst.SaveDBPath;
                SqlDbCommand sqlcommand = new SqlDbCommand(path);
                List<GroupDataModel> l = sqlcommand.DbSql<GroupDataModel>("SELECT distinct Group_number FROM UserExcel");
                if (l != null && l.Count > 0)
                {
                    List<string> sl = new List<string>();
                    foreach (GroupDataModel item in l)
                    {
                        sl.Add(item.Group_number);
                    }
                    if (sl.Count > 0)
                    {
                        if (sl.Contains(msg.req_NewGroupGetGroup.groupID))
                        {
                            msgs.rsp_NewGroupGetGroup = new Rsp_NewGroupGetGroup()
                            {
                                IsCanCreatGroup = false,
                            };
                        }
                        else
                        {
                            msgs.rsp_NewGroupGetGroup = new Rsp_NewGroupGetGroup()
                            {
                                IsCanCreatGroup = true,
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("组号查找异常！！" + ex.Message);
            }
            netServer.SendMsg(msgs);
        }
        /// <summary>
        /// 人脸更新
        /// </summary>
        /// <param name="msg"></param>
        public void Req_NewGroupUpdateUserExcel(GameMsg msg)
        {
            GameMsg gameMsg = new GameMsg()
            {
                cmd = CMD.Rsp_NewGroupUpdateUserExcel,
            };
            IsExitenceUserExcelData(GameConst.DBUserExcel);
            try
            {
                string path = Application.StartupPath + GameConst.SaveDBPath;
                SqlDbCommand sqlcommand = new SqlDbCommand(path);
                var sl = sqlcommand.Insert<UserExcel>(msg.req_NewGroupUpdateUserExcel.userExcelModes, GameConst.DBUserExcel);
                if (sl == 0)
                {
                    gameMsg.rsp_NewGroupUpdateUserExcel = new Rsp_NewGroupUpdateUserExcel() 
                    { 
                        IsSucess = 0 
                    };

                }
                else if (sl >= 1)
                {
                    gameMsg.rsp_NewGroupUpdateUserExcel = new Rsp_NewGroupUpdateUserExcel() 
                    {
                        IsSucess = 1 
                    };
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("数据插入异常" + e.Message);

            }
            netServer.SendMsg(gameMsg);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsExitenceUserExcelData(string name)
        {
            string path = Application.StartupPath + GameConst.SaveDBPath;
            SqlDbCommand sqlcommand = new SqlDbCommand(path);
            int s = sqlcommand.IsCreateTable(name);
            if (s == 0)  // 表不存在
            {
                sqlcommand.CreateTable<UserExcelMode>(name);
                sqlcommand.Dispose();
                return true;
            }
            else            // 表存在
            {
                sqlcommand.Dispose();
                return false;
            }
        }
    }
}