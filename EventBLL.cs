using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maticsoft.DBUtility;
using System.Data;

namespace _20210621
{
    public partial class EventBLL
    {
        private readonly _20210621.EventDAL dal = new _20210621.EventDAL();
        public EventBLL()
        { }
        #region  成员方法

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(_20210621.Event model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<_20210621.Event> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }


        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<_20210621.Event> DataTableToList(DataTable dt)
        {
            List<_20210621.Event> modelList = new List<_20210621.Event>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                _20210621.Event model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = dal.DataRowToModel(dt.Rows[n]);
                    if (model != null)
                    {
                        modelList.Add(model);
                    }
                }
            }
            return modelList;
        }


        #endregion  成员方法
    }
}
