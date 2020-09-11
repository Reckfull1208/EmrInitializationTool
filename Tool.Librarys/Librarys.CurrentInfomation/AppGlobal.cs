using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Librarys.CurrentInfomation
{
    public static class AppGlobal
    {
        /// <summary>
        /// 系统
        /// </summary>
        public static string CurrentPath { get; set; }

        /// <summary>
        /// 数据总路径
        /// </summary>
        public static string DataPath { get; set; } = "BaseData";
        
        /// <summary>
        /// 元素文件名
        /// </summary>
        public static string ElementFileName { get; set; } = "元素基础数据";

        /// <summary>
        /// 元素类别数据页
        /// </summary>
        public static string ElementClassifySheetName { get; set; } = "元素类别数据";

        /// <summary>
        /// 元素基础数据页
        /// </summary>
        public static string ElementBaseSheetName { get; set; } = "元素基础数据";

        /// <summary>
        /// 元素基础明细数据页
        /// </summary>
        public static string ElementDetailSheetName { get; set; } = "元素基础明细数据";

        /// <summary>
        /// 病历模板数据文件名
        /// </summary>
        public static string EmrTemplateFileName { get; set; } = "住院病历模板基础数据";

        /// <summary>
        /// 住院病历模板类别数据页
        /// </summary>

        public static string EmrTemplateClassifySheetName { get; set; } = "住院病历模板类别数据";

        /// <summary>
        /// 住院病历模板基础数据页
        /// </summary>

        public static string EmrTemplateBaseDataSheetName { get; set; } = "住院病历模板基础数据";

        /// <summary>
        /// FTS病历模板业务数据
        /// </summary>
        public static string FTSFileName { get; set; } = "FTS病历模板业务数据";

        /// <summary> 
        /// FTS病历模板业务数据页
        /// </summary>
        public static string FTSSheetName { get; set; } = "FTS病历模板业务数据";

        /// <summary>
        /// 文件总路径
        /// </summary>
        public static string FilePath { get; set; } = "Files";

        /// <summary>
        /// 文件整合路径
        /// </summary>
        public static string FileBasePath { get; set; } = "BaseFile";

        /// <summary>
        /// 数据备份路径
        /// </summary>
        public static string BackUp { get; set; } = "Backup";
        /// <summary>
        /// 文件后缀
        /// </summary>
        public static string DataFileSuffix { get; set; } = ".xlsx";

        /// <summary>
        /// 默认datatable名称
        /// </summary>
        public static string LocalDataTableName { get; set; } = "LocalDt";
    }
}
