using Librarys.CurrentInfomation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Librarys.SqlHelper
{
    public class SqlHelper
    {
        public SqlHelper()
        {

        }

        /// <summary>
        /// 查询信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="whereCommand">查询条件</param>
        /// <param name="orderCommand">排序条件</param>
        /// <param name="paras">查询字段</param>
        /// <returns></returns>
        public DataSet SelectTable(string tableName, string whereCommand = null, string orderCommand = null, List<string> paras = null, string conStr = null)
        {
            string whereString = string.IsNullOrWhiteSpace(whereCommand) ? "" : whereCommand;
            string orderString = string.IsNullOrWhiteSpace(orderCommand) ? "" : orderCommand;
            string fields = paras == null || paras.Count == 0 ? "*" : string.Join(",", paras);
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(string.IsNullOrEmpty(conStr) ? ConfigHunter.HisConnect : conStr))
                {
                    DataSet ds = new DataSet();
                    string sql = string.Format("SELECT {3} FROM {0} {1} {2}", tableName, whereString, orderString, fields);
                    try
                    {
                        sqlConnection.Open();
                        SqlDataAdapter sqlAdapter = new SqlDataAdapter(sql, sqlConnection);
                        sqlAdapter.Fill(ds, AppGlobal.LocalDataTableName);
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        sqlConnection.Close();
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 直接执行sql的方式
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public DataSet ExecSQL(string sqlStr, string conStr = null)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(string.IsNullOrWhiteSpace(conStr) ? ConfigHunter.HisConnect : conStr))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        sqlConnection.Open();
                        SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlStr, sqlConnection);
                        sqlAdapter.Fill(ds, AppGlobal.LocalDataTableName);
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        sqlConnection.Close();
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="sqlStr"></param>
        public int DeleteSql(string sqlStr, string conStr = null)
        {
            try
            { 
                using (SqlConnection sqlConnection = new SqlConnection(string.IsNullOrWhiteSpace(conStr) ? ConfigHunter.HisConnect : conStr))
                {
                    SqlTransaction tranProducts = null;
                    try
                    {
                        sqlConnection.Open();
                        tranProducts = sqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                        SqlCommand command = new SqlCommand(sqlStr, sqlConnection);
                        command.Transaction = tranProducts;
                        int i = command.ExecuteNonQuery();
                        tranProducts.Commit();
                        sqlConnection.Close();
                        return i;
                    }
                    catch (Exception ex)
                    {
                        tranProducts.Rollback();
                        sqlConnection.Close();
                        return -1;
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public bool InsertDataSetToSQL(DataSet ds, string conStr = null , int batchSize = 100000)
        {
            try
            { 
                using (SqlConnection sqlConnection = new SqlConnection(string.IsNullOrWhiteSpace(conStr) ? ConfigHunter.HisConnect : conStr))
                {
                    sqlConnection.Open();
                    SqlTransaction tran = sqlConnection.BeginTransaction(); 
                    for (int i = 0; i < ds.Tables.Count; i++) //遍历dataset
                    {
                        var dt = new DataTable();
                        dt = ds.Tables[i].Copy();
                        using (SqlBulkCopy bulk = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, tran))
                        {
                            try
                            {
                                bulk.DestinationTableName = dt.TableName;
                                bulk.BatchSize = batchSize;
                                for (int j = 0; j < dt.Columns.Count; j++) //构建sql模型
                                {
                                    var columnName = dt.Columns[j].ColumnName;
                                    bulk.ColumnMappings.Add(columnName, columnName);
                                }
                                bulk.WriteToServer(dt);
                                bulk.Close();
                            }
                            catch (Exception ex)
                            {
                                sqlConnection.Close();
                                return false;
                            }
                        }
                    }
                    tran.Commit();
                    sqlConnection.Close();
                }
                return true;
            }
            catch(Exception ext)
            {
                return false;
            }
        }

    }
}
