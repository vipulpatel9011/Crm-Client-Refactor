using System;
using System.Collections.Generic;
using System.Text;

namespace Aurea.CRM.Core.OperationHandling.Data
{
    public class LicenseInfo
    {
        public bool IsDemoVersion { get; set; }
        public bool IsEnterpriseVersion { get; set; }
    }

    public class SyncRecord
    {
        public string fullSyncTimestamp { get; set; }
        public string syncTimestamp { get; set; }        
        public string infoareaid { get; set; }
        public string datasetName { get; set; }
        public int maxRecords { get; set; }
        public int RemainingRecordCount { get; set; }
        public int rowCount { get; set; }
        public List<List<object>> metainfo { get; set; }
        public List<object> rows { get; set; }
    }

    public class DataModelSyncDeserializer
    {
        public LicenseInfo licenseInfo { get; set; }
        public List<SyncRecord> records { get; set; }
    }
}
