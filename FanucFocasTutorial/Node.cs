﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanucFocasTutorial
{
    class Node
    {
        public int updateId = 5;
        public string nodeId = "ns=2;s=cnc362.cnc362.Cycle_Counter_Shift_SL";
        public string name = "Cycle_Counter_Shift_SL";
        public string plexus_Customer_No = "310507";
        public string pcn = "Avilla";
        public string workcenter_Key = "61314";
        public string workcenter_Code = "Honda Civic cnc 359 362";
        public string cnc = "362";
        public int value = 21;

    }
}
/*
 * 
 * Product product = new Product();
product.Name = "Apple";
product.Expiry = new DateTime(2008, 12, 28);
product.Sizes = new string[] { "Small" };

                string test = @"{
                  'updateId': 5,
                  'nodeId': 'ns=2;s=cnc362.cnc362.Cycle_Counter_Shift_SL',
                  'name':'Cycle_Counter_Shift_SL',
                  'plexus_Customer_No':'310507',
                  'pcn': 'Avilla',
                  'workcenter_Key': '61314',
                  'workcenter_Code': 'Honda Civic cnc 359 362',  
                  'cnc': '362',
                  'value': 0,
                  'transDate': '2020-06-29 00:00:00'
                }"; 
JArray array = new JArray();
array.Add("Manual text");
array.Add(new DateTime(2000, 5, 23));

JObject o = new JObject();
o["MyArray"] = array;

string json = o.ToString();
// {
//   "MyArray": [
//     "Manual text",
//     "2000-05-23T00:00:00"
//   ]
// }

 */
