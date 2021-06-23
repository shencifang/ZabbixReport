using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;

namespace _20210621
{
    public partial class EventDAL
    {
        Log log = new Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");

        public EventDAL()
        { }

        #region  成员方法
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(_20210621.Event model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into event(");
            strSql.Append("id,triggerid,time,ip,content,gm,recetime,close,closetime,cause)");
            strSql.Append(" values (");
            strSql.Append("@id,@triggerid,@time,@ip,@content,@gm,@recetime,@close,@closetime,@cause)");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@id", MySqlDbType.VarChar,255),
                    new MySqlParameter("@triggerid", MySqlDbType.VarChar,255),
                    new MySqlParameter("@time", MySqlDbType.DateTime,255),
                    new MySqlParameter("@ip", MySqlDbType.VarChar,255),
                    new MySqlParameter("@content", MySqlDbType.VarChar,255),
                    new MySqlParameter("@gm", MySqlDbType.VarChar,255),
                    new MySqlParameter("@recetime", MySqlDbType.DateTime,255),
                    new MySqlParameter("@close", MySqlDbType.Double,255),
                    new MySqlParameter("@closetime", MySqlDbType.DateTime,255),
                    new MySqlParameter("@cause", MySqlDbType.VarChar,255)};
            parameters[0].Value = model.id;
            parameters[1].Value = model.triggerid;
            parameters[2].Value = model.time;
            parameters[3].Value = model.ip;
            parameters[4].Value = model.content;
            parameters[5].Value = model.gm;
            parameters[6].Value = model.recetime;
            parameters[7].Value = model.close;
            parameters[8].Value = model.closetime;
            parameters[9].Value = model.cause;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public _20210621.Event DataRowToModel(DataRow row)
        {
            _20210621.Event model = new _20210621.Event();
            if (row != null)
            {
                if (row["id"] != null && row["id"].ToString() != "")
                {
                    model.id = row["id"].ToString();
                }
                if (row["triggerid"] != null && row["triggerid"].ToString() != "")
                {
                    model.triggerid = row["triggerid"].ToString();
                }
                if (row["time"] != null && row["time"].ToString() != "")
                {
                    model.time = Convert.ToDateTime(row["time"]);
                }
                if (row["content"] != null && row["content"].ToString() != "")
                {
                    model.content = row["content"].ToString();
                }
                if (row["ip"] != null)
                {
                    model.ip = row["ip"].ToString();
                }
                if (row["gm"] != null && row["gm"].ToString() != "")
                {
                    model.gm = row["gm"].ToString();
                }
                if (row["recetime"] != null && row["recetime"].ToString() != "")
                {
                    model.recetime = Convert.ToDateTime(row["recetime"]);
                }
                if (row["close"] != null && row["close"].ToString() != "")
                {
                    model.close = Convert.ToDecimal(row["close"].ToString());
                }
                if (row["closetime"] != null && row["closetime"].ToString() != "")
                {
                    model.closetime = Convert.ToDateTime(row["closetime"]);
                }
                if (row["cause"] != null && row["cause"].ToString() != "")
                {
                    model.cause = row["cause"].ToString();
                }
            }
            return model;
        }


        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,triggerid,time,ip,content,gm,recetime,close,closetime,cause ");
            strSql.Append(" FROM event ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            log.log(strSql.ToString());
            return DbHelperMySQL.Query(strSql.ToString());
        }
        public DataSet GetList1(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,cause ");
            strSql.Append(" FROM event ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            log.log(strSql.ToString());
            return DbHelperMySQL.Query(strSql.ToString());
        }

        #endregion  成员方法

    }
}