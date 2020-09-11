using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HWKJ.Common.Serializable;
using Librarys.CurrentInfomation;
using Librarys.FTS;
using Librarys.Model;
using Librarys.OfficeHelper;
using Librarys.SqlHelper;
using Microsoft.Win32;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EmrInitializationTool.ViewModel
{
    public class EmrExportViewModel : ViewModelBase
    {
        public EmrExportViewModel()
        {
            Messenger.Default.Register<ModernProgressRing>(this, "Emrport", result =>
            {
                //ring = result;
                Init();
            }); 
        }

        #region 定义
        SqlHelper sql;
        ModernProgressRing ring;
        int count = -1;
        /// <summary>
        /// 自增序列
        /// </summary>
        private long Indexes = 1;
        private string FirstStr = "2000";
        private string MidStr = "1000";
        private char fillChar = '0';
        private int fillLength = 16;
        private ObservableCollection<EmrTemplateClassifyEx> _templateClassify;
        /// <summary>
        /// 病历模板类别
        /// </summary>
        public ObservableCollection<EmrTemplateClassifyEx> TemplateClassify
        {
            get { return _templateClassify; }
            set
            {
                _templateClassify = value;
                RaisePropertyChanged(() => TemplateClassify);
            }
        }

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
                if (value != null)
                {
                    GetEmrTempData(value.GUID);
                }
                RaisePropertyChanged(() => HospitalSelected);
            }
        }

        private EmrTemplateClassifyEx _tempClassSelected;
        /// <summary>
        /// 模板类别选中
        /// </summary>
        public EmrTemplateClassifyEx TempClassSelected
        {
            get { return _tempClassSelected; }
            set
            {
                _tempClassSelected = value;
                RaisePropertyChanged(() => TempClassSelected);
            }
        }

        private ObservableCollection<T_Base_EMRTemplate> _emrTemplate;
        public ObservableCollection<T_Base_EMRTemplate> EmrTemplate
        {
            get { return _emrTemplate; }
            set
            {
                _emrTemplate = value;
                RaisePropertyChanged(() => EmrTemplate);
            }
        }

        private RelayCommand _treeViewSelectionCommand;
        private RelayCommand _exportBaseDataCommand;
        private RelayCommand _exportAllFileCommand;
        #endregion

        #region 方法
        private void Init()
        {
            sql = new SqlHelper();
            InitPage();
        }

        private void InitPage()
        {
            GetHosInformation();
            GetEmrTemplate();
        }

        /// <summary>
        /// 获取模板信息
        /// </summary>
        private void GetEmrTempData(string hosId)
        {
            string wherePara = string.Format("where Category<300 and  isdelete = 0 and (Hos_ID='{0}' or Hos_ID is null)", hosId);
            string orderPara = "order by sortnumber asc";
            List<EmrTemplateClassifyEx> exTempList = new List<EmrTemplateClassifyEx>();
            var list = Library.Common.DataTableConvert.ConvertToModel<T_Base_EMRTemplateClassify>(sql.SelectTable("T_Base_EMRTemplateClassify", wherePara, orderPara).Tables[AppGlobal.LocalDataTableName]).ToList();
            foreach (var item in list)
            {
                EmrTemplateClassifyEx exTemp = new EmrTemplateClassifyEx()
                {
                    GUID = item.GUID,
                    Name = item.Name,
                    FatherId = item.FatherID,
                    SortNumber = item.SortNumber,
                    Category = item.Category
                };
                exTempList.Add(exTemp);
            }
            exTempList.Insert(0, new EmrTemplateClassifyEx() { GUID = null, FatherId = "guid", Name = "全部", IsSelected = true });
            TemplateClassify = new ObservableCollection<EmrTemplateClassifyEx>(GetTrees("guid", exTempList));
            GetEmrTemplate();
        }

        /// <summary>
        /// 获取院区数据
        /// </summary>
        private void GetHosInformation()
        {
            string wherePara = "where Enable = 1";
            var list = Library.Common.DataTableConvert.ConvertToModel<T_Core_Hospital>(sql.SelectTable("T_Core_Hospital", wherePara).Tables[AppGlobal.LocalDataTableName]).ToList();
            Hospital = new ObservableCollection<T_Core_Hospital>(list);
            if (Hospital.Count > 0) HospitalSelected = Hospital.FirstOrDefault();
        }

        /// <summary>
        /// 获取模板数据
        /// </summary>
        private void GetEmrTemplate(string category = null)
        {
            var categoryStr = string.IsNullOrWhiteSpace(category) ? string.Empty : string.Format("and Classify_ID='{0}'", category);
            string wherePara = string.Format("where isdelete=0 and enable=1 and file_id is not null and range=0 and (hos_id='{0}' or hos_id is null) {1}", HospitalSelected.GUID, categoryStr);
            string orderPara = "order by sortnumber asc";
            var list = Library.Common.DataTableConvert.ConvertToModel<T_Base_EMRTemplate>(sql.SelectTable("T_Base_EMRTemplate", wherePara, orderPara).Tables[AppGlobal.LocalDataTableName]).ToList();
            EmrTemplate = new ObservableCollection<T_Base_EMRTemplate>(list);
        }

        /// <summary>
        /// 递归生成树形数据
        /// </summary>
        /// <param name="delst"></param>
        /// <returns></returns>
        public List<EmrTemplateClassifyEx> GetTrees(string parentid, List<EmrTemplateClassifyEx> nodes)
        {
            List<EmrTemplateClassifyEx> mainNodes = nodes.Where(x => x.FatherId == parentid).ToList<EmrTemplateClassifyEx>();
            List<EmrTemplateClassifyEx> otherNodes = nodes.Where(x => x.FatherId != parentid).ToList<EmrTemplateClassifyEx>();
            foreach (EmrTemplateClassifyEx item in mainNodes)
            {
                item.Childs = GetTrees(item.GUID, otherNodes);
            }
            return mainNodes;
        }

        /// <summary>
        /// 新GUID
        /// </summary>
        /// <returns></returns>
        private string GetCodexStr()
        {
            string str = FirstStr + MidStr + Indexes.ToString().PadLeft(fillLength, fillChar);
            Indexes++;
            return str;
        }

        /// <summary>
        /// 导出元素类别表，基础表，明细表数据
        /// </summary>
        private void ExportElementClassiy()
        {
            var wherePara = string.Format("WHERE Enable=1 AND IsDelete=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL )", HospitalSelected.GUID);
            var orderPara = "ORDER BY SortNumber ASC ";
            var dt = sql.SelectTable("T_Base_EMRElementClassify", wherePara, orderPara).Tables[AppGlobal.LocalDataTableName];
            var dtBase = sql.SelectTable("T_Base_EMRElement", wherePara, orderPara).Tables[AppGlobal.LocalDataTableName];
            var dtDetail = sql.SelectTable("T_Base_EMRElementDetail", wherePara, orderPara).Tables[AppGlobal.LocalDataTableName];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var oldClassifyGuid = dt.Rows[i]["GUID"].ToString();
                var baseWithOld = dtBase.Select("Classify_ID = '" + oldClassifyGuid + "'");
                var newGuid = GetCodexStr();
                dt.Rows[i]["GUID"] = newGuid;
                dt.Rows[i]["Hos_ID"] = null;
                foreach (var dtb in baseWithOld)
                {
                    int indexdtCode = dtBase.Rows.IndexOf(dtb);
                    dtBase.Rows[indexdtCode]["Classify_ID"] = newGuid;
                }
            }

            for (int i = 0; i < dtBase.Rows.Count; i++)
            {
                var oldElementGuid = dtBase.Rows[i]["GUID"].ToString();
                var detailWithOld = dtDetail.Select("Element_ID = " + "'" + oldElementGuid + "'");
                var newGuid = GetCodexStr();
                dtBase.Rows[i]["GUID"] = newGuid;
                dtBase.Rows[i]["Hos_ID"] = null;
                foreach (var item in detailWithOld)
                {
                    int indexCode = dtDetail.Rows.IndexOf(item);
                    dtDetail.Rows[indexCode]["Element_ID"] = newGuid;
                }
            }

            for (int i = 0; i < dtDetail.Rows.Count; i++)
            {
                dtDetail.Rows[i]["GUID"] = GetCodexStr();
                dtDetail.Rows[i]["Hos_ID"] = null;
            }

            var filepath = Path.Combine(AppGlobal.CurrentPath, AppGlobal.DataPath);

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
                    count = excelHelper.DataSetToExcel(ds, true);
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
            var wherePara = string.Format("WHERE Category<300  AND IsDelete=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL )", HospitalSelected.GUID);
            var orderPara = "ORDER BY SortNumber ASC ";
            var dt = sql.SelectTable("T_Base_EMRTemplateClassify", wherePara, orderPara).Tables[AppGlobal.LocalDataTableName];
            wherePara = string.Format("WHERE IsDelete = 0 and Category<300 AND Enable = 1 AND FILE_ID IS NOT NULL AND Range=0 AND (Hos_ID = '{0}' OR Hos_ID IS NULL )", HospitalSelected.GUID);
            var dtTemp = sql.SelectTable("T_Base_EMRTemplate", wherePara, orderPara).Tables[AppGlobal.LocalDataTableName];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var oldClassifyGuid = dt.Rows[i]["GUID"].ToString();
                var tempWithOldGuid = dtTemp.Select("Classify_ID = '" + oldClassifyGuid + "'");
                var newGuid = GetCodexStr();
                dt.Rows[i]["GUID"] = newGuid;
                dt.Rows[i]["Hos_ID"] = null;
                foreach (var item in tempWithOldGuid)
                {
                    int indexCode = dtTemp.Rows.IndexOf(item);
                    dtTemp.Rows[indexCode]["Classify_ID"] = newGuid;
                }
            }

            for (int i = 0; i < dtTemp.Rows.Count; i++)
            {
                dtTemp.Rows[i]["GUID"] = GetCodexStr();
                dtTemp.Rows[i]["Hos_ID"] = null;
            }

            //路径为根目录 BaseData 文件夹  文件名为基础数据
            var filepath = Path.Combine(AppGlobal.CurrentPath, AppGlobal.DataPath);
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
                    count += excelHelper.DataSetToExcel(ds, true);
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
            var sqlStr = string.Format(@"SELECT a.* FROM RHISCache.dbo.T_Glob_FileUpload a 
                                        INNER JOIN dbo.T_Base_EMRTemplate b ON a.GUID=b.File_ID
                                        WHERE b.IsDelete = 0 and b.Category<300 AND b.Enable = 1 AND b.FILE_ID IS NOT NULL AND b.Range=0 AND (b.Hos_ID = '{0}' OR b.Hos_ID IS NULL )", HospitalSelected.GUID);
            var dt = sql.ExecSQL(sqlStr).Tables[AppGlobal.LocalDataTableName];
            //路径为根目录 BaseData 文件夹  文件名为基础数据 
            var filepath = Path.Combine(AppGlobal.CurrentPath, AppGlobal.DataPath);
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
                    count += excelHelper.DataTableToExcel(dt, true);
                    excelHelper.Dispose();
                }
            }
            catch
            {
                ModernDialog.ShowMessage($"关闭文件{AppGlobal.FTSFileName + AppGlobal.DataFileSuffix}后再试", "ERROR", System.Windows.MessageBoxButton.OK);
            }

        }

        /// <summary>
        /// 导出所有可用模板文件
        /// 导出文件分多份
        /// 总文件一份，编译拷贝到服务器不做任何处理
        /// 若干类别分类文件，名字更换为模板名称
        /// 导出时需要清空对应大文件夹下所有内容
        /// </summary>
        private void ExportEmrFiles()
        {
            string sqlStr = string.Format(@"SELECT a.File_ID,b.Name,a.Name AS TempName FROM dbo.T_Base_EMRTemplate a
                                INNER JOIN dbo.T_Base_EMRTemplateClassify b ON a.Classify_ID = b.GUID
                                WHERE a.IsDelete = 0 AND a.Enable = 1 AND a.FILE_ID IS NOT NULL 
                                AND a.Range=0 AND (a.Hos_ID = '{0}' OR a.Hos_ID IS NULL )", HospitalSelected.GUID);
            var dt = sql.ExecSQL(sqlStr).Tables[AppGlobal.LocalDataTableName];
            var filepath = Path.Combine(AppGlobal.CurrentPath, AppGlobal.FilePath);

            DeleteDir(filepath);

            //由于现有下载方式未支持批量下载；所以采用循环下载方式
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var model = new FTSModel()
                {
                    FileId = dt.Rows[i]["File_ID"].ToString(),
                    Name = dt.Rows[i]["TempName"].ToString(),
                    ClassifyName = dt.Rows[i]["Name"].ToString(),
                    FileDirectory = filepath,
                    IsNeedToClassification = true
                };
                UploadDownHelper.DownLoad(JsonConvert.NewtonsoftJsonConvert(model));
            }


        }

        #region 直接删除指定目录下的所有文件及文件夹(保留目录)

        public void DeleteDir(string file)
        {
            try
            {
                //去除文件夹和子文件的只读属性
                //去除文件夹的只读属性
                System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
                fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;

                //去除文件的只读属性
                System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);

                //判断文件夹是否还存在
                if (Directory.Exists(file))
                {
                    foreach (string f in Directory.GetFileSystemEntries(file))
                    {
                        if (File.Exists(f))
                        {
                            //如果有子文件删除文件
                            File.Delete(f);
                            Console.WriteLine(f);
                        }
                        else
                        {
                            //循环递归删除子文件夹
                            DeleteDir(f);
                        }
                    }

                    //删除空文件夹
                    Directory.Delete(file);
                    Console.WriteLine(file);
                }

            }
            catch (Exception ex) // 异常处理
            {
                Console.WriteLine(ex.Message.ToString());// 异常信息
            }
        }
        #endregion

        #endregion

        #region 命令

        /// <summary>
        /// 模板类型选中
        /// </summary>
        public RelayCommand TreeViewSelectionCommand
        {
            get
            {
                if (_treeViewSelectionCommand == null)
                {
                    _treeViewSelectionCommand = new RelayCommand(() =>
                    {
                        GetEmrTemplate(TempClassSelected.GUID);
                    });
                }
                return _treeViewSelectionCommand;
            }
            set
            {
                _treeViewSelectionCommand = value;
            }
        }

        /// <summary>
        /// 导出基础数据
        /// 包含表有：
        /// T_Base_EMRElementClassify --元素类型表
        /// T_Base_EMRElement -- 元素基础表
        /// T_Base_EMRElementDetail -- 多级元素子表
        /// T_Base_EMRTemplateClassify --模板类型
        /// T_Base_EMRTemplate --模板基础表
        /// 导出后重新遍历所有相关数据 修改GUID关联 (待定)
        /// 导出后的数据将清空院区ID 作为基础数据
        /// </summary>
        public RelayCommand ExportBaseDataCommand
        {
            get
            {
                if (_exportBaseDataCommand == null)
                {
                    _exportBaseDataCommand = new RelayCommand(() =>
                    {
                        //ring.Visibility = System.Windows.Visibility.Visible;
                        Task.Factory.StartNew(new Action(() =>
                        {
                            ring.Dispatcher.BeginInvoke(new Action(() =>
                            { 
                                ExportElementClassiy();
                                ExportTempData();
                                ExportFTSData();
                               // ring.Visibility = System.Windows.Visibility.Hidden;
                                if (count > -1)
                                {
                                    ModernDialog.ShowMessage("导出成功", "提示", System.Windows.MessageBoxButton.OK);
                                }
                                else
                                {
                                    ModernDialog.ShowMessage("导出失败", "提示", System.Windows.MessageBoxButton.OK);
                                }
                            }));
                        }));

                        
                    });
                }
                return _exportBaseDataCommand;
            }
            set
            {
                _exportBaseDataCommand = value;
            }
        }

        /// <summary>
        /// 导出所有文件
        /// </summary>
        public RelayCommand ExportAllFileCommand
        {
            get
            {
                if (_exportAllFileCommand == null)
                {
                    _exportAllFileCommand = new RelayCommand(() =>
                    {
                        ExportEmrFiles();
                    });
                }
                return _exportAllFileCommand;
            }
            set
            {
                _exportAllFileCommand = value;
            }
        }

        #endregion

    }
}
