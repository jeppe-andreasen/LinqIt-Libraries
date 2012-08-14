//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web.Security;
//using System.Web.UI.WebControls;
//using LinqIt.Components;
//using umbraco;
//using umbraco.cms.businesslogic.datatype;

//namespace LinqIt.UmbracoCustomFieldTypes
//{
//    public class LayoutGrid : AbstractDataEditor
//    {
//        protected LinqItGridEditor _control = new LinqItGridEditor();

//        public LayoutGrid()
//        {
//            _control.Init += new EventHandler(control_Init); 
//            RenderControl = _control; 
//            DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(DataEditorControl_OnSave);
//        } 
        
//        public override Guid Id
//        {
//            get { return new Guid("bdbe818d-e088-4caa-9cb8-087494ce73ae"); }
//        } 
        
//        public override string DataTypeName
//        {
//            get { return "LinqIt Layout Grid"; }
//        } 
        
//        protected void control_Init(object sender, EventArgs e)
//        {
//            _control.Value = base.Data.Value as string;
//            _control.Layout = LayoutDefinition;
//            _control.ModuleTreeProvider = typeof(TestProvider).FullName;
//        } 
        
//        protected void DataEditorControl_OnSave(EventArgs e)
//        {
//            base.Data.Value = _control.Value;
//        }

//        [DataEditorSetting("Layout Definition", description = "The layout definition", defaultValue = "")]
//        public string LayoutDefinition { get; set; }
//    }
//}
