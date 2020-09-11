using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Librarys.CurrentInfomation;
using Librarys.Model;
using Librarys.OfficeHelper;
using Librarys.SqlHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace EmrInitializationTool.ViewModel
{
    public class EmrInportViewModel : ViewModelBase
    {
        public EmrInportViewModel()
        {
            Init();
        }

        #region 定义
        private DataTable _dtElementClassify;
        /// <summary>
        /// 类型
        /// </summary>
        public DataTable DtElementClassify
        {
            get { return _dtElementClassify; }
            set
            {
                _dtElementClassify = value;
                RaisePropertyChanged(() => _dtElementClassify);
            }
        }

        private DataTable _dtElementBase;
        /// <summary>
        /// 基础数据
        /// </summary>
        public DataTable DtElementBase
        {
            get { return _dtElementBase; }
            set
            {
                _dtElementBase = value;
                RaisePropertyChanged(() => DtElementBase);
            }
        }

        private DataTable _dtElementDetal;
        /// <summary>
        /// 明细
        /// </summary>
        public DataTable DtElementDetal
        {
            get { return _dtElementDetal; }
            set
            {
                _dtElementDetal = value;
                RaisePropertyChanged(() => DtElementDetal);
            }
        }

        private DataTable _dtEmrTemplateClassify;
        /// <summary>
        /// 病历模板类别
        /// </summary>
        public DataTable DtEmrTemplateClassify
        {
            get { return _dtEmrTemplateClassify; }
            set
            {
                _dtEmrTemplateClassify = value;
                RaisePropertyChanged(() => DtEmrTemplateClassify);
            }
        }

        private DataTable _dtEmrTemplateBase;
        /// <summary>
        /// 病历模板数据
        /// </summary>
        public DataTable DtEmrTemplateBase
        {
            get { return _dtEmrTemplateBase; }
            set
            {
                _dtEmrTemplateBase = value;
                RaisePropertyChanged(() => DtEmrTemplateBase);
            }
        }

        private DataTable _dtFtsData;
        /// <summary>
        /// 文件业务数据
        /// </summary>
        public DataTable DtFtsData
        {
            get { return _dtFtsData; }
            set
            {
                _dtFtsData = value;
                RaisePropertyChanged(() => DtFtsData);
            }
        }

        private Visibility _visiElement;
        public Visibility VisiElement
        {
            get { return _visiElement; }
            set
            {
                _visiElement = value;
                RaisePropertyChanged(() => VisiElement);
            }
        }

        private Visibility _visiTemplate;
        public Visibility VisiTemplate
        {
            get { return _visiTemplate; }
            set
            {
                _visiTemplate = value;
                RaisePropertyChanged(() => VisiTemplate);
            }
        }

        private Visibility _visiFts;
        public Visibility VisiFts
        {
            get { return _visiFts; }
            set
            {
                _visiFts = value;
                RaisePropertyChanged(() => VisiFts);
            }
        }

        private string DatetimeStr = string.Empty;

        private ObservableCollection<T_Core_Hospital> _hospital;
        /// <summary>
        /// 院区信息
        /// </summary>
        public ObservableCollection<T_Core_Hospital> Hospital
        {
            get { return _hospital; }
            set
            {
                _hospital = value;
                RaisePropertyChanged(() => Hospital);
            }
        }

        private T_Core_Hospital _hospitalSelected;
        /// <summary>
        /// 医院选中
        /// </summary>
        public T_Core_Hospital HospitalSelected
        {
            get { return _hospitalSelected; }
            set
            {
                _hospitalSelected = value;
                RaisePropertyChanged(() => HospitalSelected);
            }
        }

        private RelayCommand _lVElementCommand;
        private RelayCommand _lVEmrCommand;
        private RelayCommand _lVFileCommand;
        private RelayCommand _inportCommand;
        #endregion

        #region 方法
        private void Init()
        {
            VisiElement = Visibility.Visible;
            VisiTemplate = Visibility.Collapsed;
            VisiFts = Visibility.Collapsed;
            using (ExcelHelper excelHelper = new ExcelHelper(Path.Combine(AppGlobal.CurrentPath, AppGlobal.DataPath, AppGlobal.ElementFileName + AppGlobal.DataFileSuffix)))//定义一个范围，在范围结束时处理对象  
            {
                DtElementClassify = new DataTable();
                DtElementClassify = excelHelper.ExcelToDataTable(AppGlobal.ElementClassifySheetName, true);

                DtElementBase = new DataTable();
                DtElementBase = excelHelper.ExcelToDataTable(AppGlobal.ElementBaseSheetName, true);

                DtElementDetal = new DataTable();
                DtElementDetal = excelHelper.ExcelToDataTable(AppGlobal.ElementDetailSheetName, true);
            }

            using (ExcelHelper excelHelper = new ExcelHelper(Path.Combine(AppGlobal.CurrentPath, AppGlobal.DataPath, AppGlobal.EmrTemplateFileName + AppGlobal.DataFileSuffix)))//定义一个范围，在范围结束时处理对象  
            {
                DtEmrTemplateClassify = new DataTable();
                DtEmrTemplateClassify = excelHelper.ExcelToDataTable(AppGlobal.EmrTemplateClassifySheetName, true);

                DtEmrTemplateBase = new DataTable();
                DtEmrTemplateBase = excelHelper.ExcelToDataTable(AppGlobal.EmrTemplateBaseDataSheetName, true);
            }

            using (ExcelHelper excelHelper = new ExcelHelper(Path.Combine(AppGlobal.CurrentPath, AppGlobal.DataPath, AppGlobal.FTSFileName + AppGlobal.DataFileSuffix)))//定义一个范围，在范围结束时处理对象  
            {
                DtFtsData = new DataTable();
                DtFtsData = excelHelper.ExcelToDataTable(AppGlobal.FTSSheetName, true);
            }
            GetHosInformation();
        }
        /// <summary>
        /// 获取院区数据
        /// </summary>
        private void GetHosInformation()
        {
            string wherePara = "where Enable = 1";
            var list = Library.Common.DataTableConvert.ConvertToModel<T_Core_Hospital>(new SqlHelper().SelectTable("T_Core_Hospital", wherePara, null, null, ConfigHunter.testSqlCon).Tables[AppGlobal.LocalDataTableName]).ToList();
            Hospital = new ObservableCollection<T_Core_Hospital>(list);
            if (Hospital.Count > 0) HospitalSelected = Hospital.FirstOrDefault();
        }
        /// <summary>
        /// 导出元素类别表，基础表，明细表数据
        /// </summary>
        private void ExportElementClassiy()
        {
            var sql = new SqlHelper();
            var wherePara = string.Format("WHERE Enable=1 AND IsDelete=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL )", HospitalSelected?.GUID);
            var orderPara = "ORDER BY SortNumber ASC ";
            var dt = sql.SelectTable("T_Base_EMRElementClassify", wherePara, orderPara, null, ConfigHunter.testSqlCon).Tables[AppGlobal.LocalDataTableName];
            var dtBase = sql.SelectTable("T_Base_EMRElement", wherePara, orderPara, null, ConfigHunter.testSqlCon).Tables[AppGlobal.LocalDataTableName];
            var dtDetail = sql.SelectTable("T_Base_EMRElementDetail", wherePara, orderPara, null, ConfigHunter.testSqlCon).Tables[AppGlobal.LocalDataTableName];

            var filepath = Path.Combine(AppGlobal.CurrentPath, AppGlobal.BackUp, DatetimeStr);

            if (false == System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory(filepath);
            }
            var fullPath = Path.Combine(filepath, AppGlobal.ElementFileName + AppGlobal.DataFileSuffix);
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                using (ExcelHelper excelHelper = new ExcelHelper(fullPath))
                {
                    dt.TableName = AppGlobal.ElementClassifySheetName;
                    dtBase.TableName = AppGlobal.ElementBaseSheetName;
                    dtDetail.TableName = AppGlobal.ElementDetailSheetName;
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt.Copy());
                    ds.Tables.Add(dtBase.Copy());
                    ds.Tables.Add(dtDetail.Copy());
                    excelHelper.DataSetToExcel(ds, true);
                    excelHelper.Dispose();
                }
            }
            catch
            {
                ModernDialog.ShowMessage($"关闭文件{AppGlobal.ElementFileName + AppGlobal.DataFileSuffix}后再试", "ERROR", System.Windows.MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 导出模板数据
        /// </summary>
        private void ExportTempData()
        {
            var wherePara = string.Format("WHERE Category<300  AND IsDelete=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL )", HospitalSelected?.GUID);
            var orderPara = "ORDER BY SortNumber ASC ";
            var dt = new SqlHelper().SelectTable("T_Base_EMRTemplateClassify", wherePara, orderPara, null, ConfigHunter.testSqlCon).Tables[AppGlobal.LocalDataTableName];
            wherePara = string.Format("WHERE Category<300  and IsDelete = 0 AND Enable = 1 AND FILE_ID IS NOT NULL AND Range=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL )", HospitalSelected?.GUID);
            var dtTemp = new SqlHelper().SelectTable("T_Base_EMRTemplate", wherePara, orderPara, null, ConfigHunter.testSqlCon).Tables[AppGlobal.LocalDataTableName];

            //路径为根目录 BaseData 文件夹  文件名为基础数据
            var filepath = Path.Combine(AppGlobal.CurrentPath, AppGlobal.BackUp, DatetimeStr);
            //每次保存存储可追溯的文件
            if (false == System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory(filepath);
            }
            var fullPath = Path.Combine(filepath, AppGlobal.EmrTemplateFileName + AppGlobal.DataFileSuffix);
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                using (ExcelHelper excelHelper = new ExcelHelper(fullPath))
                {
                    dt.TableName = AppGlobal.EmrTemplateClassifySheetName;
                    dtTemp.TableName = AppGlobal.EmrTemplateBaseDataSheetName;
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt.Copy());
                    ds.Tables.Add(dtTemp.Copy());
                    excelHelper.DataSetToExcel(ds, true);
                    excelHelper.Dispose();
                }
            }
            catch
            {
                ModernDialog.ShowMessage($"关闭文件{AppGlobal.EmrTemplateFileName + AppGlobal.DataFileSuffix}后再试", "ERROR", System.Windows.MessageBoxButton.OK);
            }


        }

        /// <summary>
        /// 导出FTS数据
        /// </summary>
        private void ExportFTSData()
        {
            var sqlStr = string.Format(@"SELECT b.* FROM dbo.T_Base_EMRTemplate a 
                                  INNER JOIN RHISCache.dbo.T_Glob_FileUpload b ON a.File_ID = b.GUID
                                  WHERE a.IsDelete = 0 AND a.Enable = 1 AND a.FILE_ID IS NOT NULL 
                                  AND a.Range=0 AND (a.Hos_ID = '{0}' OR a.Hos_ID IS NULL )", HospitalSelected?.GUID);

            var sql = new SqlHelper();

            var dt = sql.ExecSQL(sqlStr, ConfigHunter.testSqlCon).Tables[AppGlobal.LocalDataTableName];
            //路径为根目录 BaseData 文件夹  文件名为基础数据 
            var filepath = Path.Combine(AppGlobal.CurrentPath, AppGlobal.BackUp, DatetimeStr);
            //每次保存存储可追溯的文件
            if (false == System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory(filepath);
            }
            var fullPath = Path.Combine(filepath, AppGlobal.FTSFileName + AppGlobal.DataFileSuffix);
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                using (ExcelHelper excelHelper = new ExcelHelper(fullPath))//定义一个范围，在范围结束时处理对象  
                {
                    dt.TableName = AppGlobal.FTSSheetName;
                    excelHelper.DataTableToExcel(dt, true);
                    excelHelper.Dispose();
                }
            }
            catch
            {
                ModernDialog.ShowMessage($"关闭文件{AppGlobal.FTSFileName + AppGlobal.DataFileSuffix}后再试", "ERROR", System.Windows.MessageBoxButton.OK);
            }

        }

        /// <summary>
        /// 删除数据
        /// </summary>
        private int DeleteData()
        {
            string sql = string.Empty;
            sql += string.Format(@"DELETE a FROM RHISCache.dbo.T_Glob_FileUpload a 
                                        INNER JOIN dbo.T_Base_EMRTemplate b ON a.GUID=b.File_ID
                                        WHERE b.IsDelete = 0 and b.Category<300 AND b.Enable = 1 AND b.FILE_ID IS NOT NULL AND b.Range=0 AND (b.Hos_ID = '{0}' OR b.Hos_ID IS NULL )", HospitalSelected?.GUID);
            sql += string.Format(@"DELETE FROM  dbo.T_Base_EMRElementClassify WHERE Enable=1 AND IsDelete=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL );", HospitalSelected?.GUID);
            sql += string.Format(@"DELETE FROM dbo.T_Base_EMRElement WHERE Enable=1 AND IsDelete=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL );", HospitalSelected?.GUID);
            sql += string.Format(@"DELETE FROM dbo.T_Base_EMRElementDetail WHERE Enable=1 AND IsDelete=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL );", HospitalSelected?.GUID);
            sql += string.Format(@"DELETE FROM dbo.T_Base_EMRTemplateClassify WHERE Category<300  AND IsDelete=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL );", HospitalSelected?.GUID);
            sql += string.Format(@"DELETE FROM dbo.T_Base_EMRTemplate WHERE IsDelete = 0 and Category<300 AND Enable = 1 AND FILE_ID IS NOT NULL AND Range=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL );", HospitalSelected?.GUID); 
            return new SqlHelper().DeleteSql(sql, ConfigHunter.testSqlCon);
        }

        #endregion

        #region 命令
        public RelayCommand LVElementCommand
        {
            get
            {
                if (_lVElementCommand == null)
                {
                    _lVElementCommand = new RelayCommand(() =>
                    {
                        VisiElement = Visibility.Visible;
                        VisiTemplate = Visibility.Collapsed;
                        VisiFts = Visibility.Collapsed;
                    });
                }
                return _lVElementCommand;
            }
            set
            {
                _lVElementCommand = value;
            }
        }

        public RelayCommand LVEmrCommand
        {
            get
            {
                if (_lVEmrCommand == null)
                {
                    _lVEmrCommand = new RelayCommand(() =>
                    {
                        VisiElement = Visibility.Collapsed;
                        VisiTemplate = Visibility.Visible;
                        VisiFts = Visibility.Collapsed;
                    });
                }
                return _lVEmrCommand;
            }
            set
            {
                _lVEmrCommand = value;
            }
        }

        public RelayCommand LVFileCommand
        {
            get
            {
                if (_lVFileCommand == null)
                {
                    _lVFileCommand = new RelayCommand(() =>
                    {
                        VisiElement = Visibility.Collapsed;
                        VisiTemplate = Visibility.Collapsed;
                        VisiFts = Visibility.Visible;
                    });
                }
                return _lVFileCommand;
            }
            set
            {
                _lVFileCommand = value;
            }
        }

        public RelayCommand InportCommand
        {
            get
            {
                if (_inportCommand == null)
                {
                    _inportCommand = new RelayCommand(() =>
                    {
                        var fir = ConfigHunter.testSqlCon.IndexOf('=');
                        var last = ConfigHunter.testSqlCon.IndexOf(';');
                        var ipAdd = ConfigHunter.testSqlCon.Substring(fir + 1, last - fir);
                        if (ModernDialog.ShowMessage($"即将批量导入数据到{ipAdd} {HospitalSelected.Hos_Name} 院区，是否继续？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            if (ModernDialog.ShowMessage($"即将删除元素类型表、元素基础表、元素明细表、病历模板类别表、病历模板表全部数据及Cache库文档上传Emr后缀文档，请慎重，是否继续？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                DatetimeStr = DateTime.Now.ToString("yyyyMMss_HHmmss_fff");
                                //第一步 备份到备份路径
                                ExportElementClassiy();
                                ExportTempData();
                                ExportFTSData();
                                //第二步 删除相关数据
                                if( DeleteData() == -1)
                                {
                                    ModernDialog.ShowMessage("删除相关数据异常，请还原数据", "错误", MessageBoxButton.OK);
                                    return;
                                }
                                //第三步 批量写入
                                var sql = new SqlHelper();
                                var ds = new DataSet();
                                var DtElementClassifyCopy = DtElementClassify.Copy();
                                DtElementClassifyCopy.TableName = "T_Base_EMRElementClassify";
                                for (int i = 0; i < DtElementClassifyCopy.Rows.Count; i++)
                                {
                                    DtElementClassifyCopy.Rows[i]["Hos_ID"] = null;
                                }

                                var DtElementBaseCopy = DtElementBase.Copy();
                                DtElementBaseCopy.TableName = "T_Base_EMRElement";
                                for (int i = 0; i < DtElementBaseCopy.Rows.Count; i++)
                                {
                                    DtElementBaseCopy.Rows[i]["Hos_ID"] = null;
                                }
                                var DtElementDetalCopy = DtElementDetal.Copy();
                                DtElementDetalCopy.TableName = "T_Base_EMRElementDetail";
                                for (int i = 0; i < DtElementDetalCopy.Rows.Count; i++)
                                {
                                    DtElementDetalCopy.Rows[i]["Hos_ID"] = null;
                                }
                                var DtEmrTemplateClassifyCopy = DtEmrTemplateClassify.Copy();
                                DtEmrTemplateClassifyCopy.TableName = "T_Base_EMRTemplateClassify";
                                for (int i = 0; i < DtEmrTemplateClassifyCopy.Rows.Count; i++)
                                {
                                    DtEmrTemplateClassifyCopy.Rows[i]["Hos_ID"] = HospitalSelected.GUID;
                                }
                                var DtEmrTemplateBaseCopy = DtEmrTemplateBase.Copy();
                                DtEmrTemplateBaseCopy.TableName = "T_Base_EMRTemplate";
                                for (int i = 0; i < DtEmrTemplateBaseCopy.Rows.Count; i++)
                                {
                                    DtEmrTemplateBaseCopy.Rows[i]["Hos_ID"] = HospitalSelected.GUID;
                                }
                                var DtFtsDataCopy = DtFtsData.Copy();
                                DtFtsDataCopy.TableName = "RHISCache.dbo.T_Glob_FileUpload"; 
                                ds.Tables.Add(DtElementClassifyCopy);
                                ds.Tables.Add(DtElementBaseCopy);
                                ds.Tables.Add(DtElementDetalCopy);
                                ds.Tables.Add(DtEmrTemplateClassifyCopy);
                                ds.Tables.Add(DtEmrTemplateBaseCopy);
                                ds.Tables.Add(DtFtsDataCopy);
                                var bl = sql.InsertDataSetToSQL(ds, ConfigHunter.testSqlCon);
                                if (bl)
                                {
                                    ModernDialog.ShowMessage("导入成功", "", MessageBoxButton.OK);
                                }
                                else
                                {
                                    ModernDialog.ShowMessage("导入失败！！！请重试或从备份路径下使用sqlsever还原数据", "", MessageBoxButton.OK);
                                }
                            }
                        }
                    });
                }
                return _inportCommand;
            }
            set
            {
                _inportCommand = value;
            }
        }

        #endregion

    }
}
