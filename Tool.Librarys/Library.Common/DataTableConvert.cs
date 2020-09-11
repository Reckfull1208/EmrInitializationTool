using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace Library.Common
{
    /// <summary>
    /// DataTable转化类
    /// </summary>
    public class DataTableConvert
    {
        public static IList<T> ConvertToModel<T>(DataTable dt) where T : new()
        {
            // 定义集合    
            IList<T> ts = new List<T>();

            // 获得此模型的类型   
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();

                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    var tempName = pi.Name;
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite)
                            continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);

                    }
                }
                ts.Add(t);
            }
            return ts;
        }

        #region  DataSet转换为Json

        public static string DataSetToJson(DataSet dataSet)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                jsonString += "\"" + table.TableName + "\":" + ToJson(table) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }

        #endregion

        #region Datatable转换为Json

        public static string ToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = String.Format(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }

        public static string ToJson(DataTable dt, string jsonName)
        {
            StringBuilder json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
                jsonName = dt.TableName;
            json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        json.Append("\"" + dt.Columns[j].ColumnName + "\":" + String.Format(dt.Rows[i][j].ToString(), type));
                        if (j < dt.Columns.Count - 1)
                        {
                            json.Append(",");
                        }
                    }
                    json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        json.Append(",");
                    }
                }
            }
            json.Append("]}");
            return json.ToString();
        }

        #endregion

        /// <summary>
        /// 将JSON解析成DataSet只限标准的JSON数据
        /// 例如：Json＝{t1:[{name:'数据name',type:'数据type'}]} 
        /// 或 Json＝{t1:[{name:'数据name',type:'数据type'}],t2:[{id:'数据id',gx:'数据gx',val:'数据val'}]}
        /// </summary>
        /// <param name="json">Json字符串</param>
        /// <returns>DataSet</returns>
        public static DataSet JsonToDataSet(string json)
        {
            try
            {
                DataSet ds = new DataSet();
                JavaScriptSerializer jss = new JavaScriptSerializer();


                object obj = jss.DeserializeObject(json);
                Dictionary<string, object> datajson = (Dictionary<string, object>)obj;


                foreach (var item in datajson)
                {
                    DataTable dt = new DataTable(item.Key);
                    object[] rows = (object[])item.Value;
                    foreach (var row in rows)
                    {
                        Dictionary<string, object> val = (Dictionary<string, object>)row;
                        DataRow dr = dt.NewRow();
                        var i = 0;
                        foreach (KeyValuePair<string, object> sss in val)
                        {
                            if (sss.Value == null || string.IsNullOrEmpty(sss.Value.ToString()))
                            {
                                i++;
                            }

                            if (!dt.Columns.Contains(sss.Key))
                            {
                                dt.Columns.Add(sss.Key);
                                dr[sss.Key] = sss.Value;
                            }
                            else
                                dr[sss.Key] = sss.Value;
                        }

                        if (i != val.Count)
                        {
                            dt.Rows.Add(dr);
                        }
                    }
                    ds.Tables.Add(dt);
                }

                return ds;
            }
            catch
            {
                return null;
            }
        }

    }
}
