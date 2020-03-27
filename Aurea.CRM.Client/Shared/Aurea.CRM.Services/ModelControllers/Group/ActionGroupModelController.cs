using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Aurea.CRM.Core.Configuration;
using Aurea.CRM.Core.CRM.DataModel;
using Aurea.CRM.Core.CRM.Delegates;
using Aurea.CRM.Core.CRM.Features;
using Aurea.CRM.Core.CRM.UIModel;
using Aurea.CRM.Core.Extensions;
using Aurea.CRM.Core.OfflineStorage;
using Aurea.CRM.Core.OperationHandling.Data;
using Aurea.CRM.Core.Session;
using Aurea.CRM.Core.Utilities;
using Aurea.CRM.Services.Delegates;
using Aurea.CRM.UIModel;
using Aurea.CRM.UIModel.Fields;
using Aurea.CRM.UIModel.Groups;

namespace Aurea.CRM.Services.ModelControllers.Group
{
    public class UPActionGroupModelController : UPMenuGroupModelController, UPRecordCopyDelegate, UPOfflineRequestDelegate, UPExecuteWorkflowRequestDelegate
    {
        protected IIdentifier _identifier;
        protected UPRecordCopy recordCopy;
        protected UPOfflineRecordRequest offlineRequest;
        protected UPExecuteWorkflowServerOperation workFlowRequest;
        protected UPMTriggerActionGroup triggerActionGroup;
        protected string outputValue;
        protected bool ignoreAction;
        public string LinkRecordIdentification { get; private set; }

        public string ActionConfig { get; private set; }

        public string ActionType { get; private set; }

        public List<string> ResultMessages { get; private set; }

        public UPActionGroupModelController(FormItem _formItem, IIdentifier identifier, IGroupModelControllerDelegate _theDelegate):base(_theDelegate)
        {
            this._identifier = identifier;
        }

        void PerformWorkflowAction(object sender)
        {
            if (this.ignoreAction)
            {
                return;
            }

            this.ShowWorking();
            List<string> records = null;
            List<object> parameters = this.WorkflowParameters();
            string workflowName = this.FormItem.ViewReference.ContextValueForKey("Func3");
            string workflowFlags = this.FormItem.Options.ValueOrDefault("Flags") as string;
            if (string.IsNullOrEmpty(workflowFlags))
            {
                workflowFlags = "768";
            }

            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                records = new List<string>() { this.LinkRecordIdentification };
            }

            Dictionary<string, object> additionalRecords = this.ParameterDictionary(); // this.ParameterDictionary()[".links"];   TODO : need to fix


            foreach (string _recordIdentification in additionalRecords.Values)
            {
                if (!_recordIdentification.IsRecordIdentification())
                {
                    continue;
                }

                if (records == null)
                {
                    records.Add(_recordIdentification);
                }
                else
                {
                    records.Add(_recordIdentification);
                }
            }
            
           this.workFlowRequest = new UPExecuteWorkflowServerOperation(workflowName, records, parameters, workflowFlags, this);
            ServerSession.CurrentSession.ExecuteRequest(this.workFlowRequest);
        }

        public Dictionary<string, object> ParameterDictionary()
        { 
            string paramSource = this.FormItem.ViewReference.ContextValueForKey("Func0");
            return paramSource.StartsWith("$") ? this.DataFromValueName(paramSource.Substring(1)) : null;
        }

        public List<object> WorkflowParameters()
        {
            Dictionary<string, object> paramDict = this.ParameterDictionary();

            if (paramDict.Count == 0)
            {
                return null;
            }

            List<object> parameterArray = new List<object>();

            foreach (string parameter in paramDict.Keys)
            {
                if (parameter.StartsWith("."))
                {
                    continue;
                }

                Dictionary<string, object> dict = new Dictionary<string, object>()
                { 
                    { "Id", 0 },
                    { "Name", parameter},
                    { "Values", paramDict[parameter] }
                };
                parameterArray.Add(dict);
            }
            return parameterArray;
        }

        void PerformCopyRecordAction(object sender)
        {
            if (this.ignoreAction)
            {
                return;
            }

            this.ShowWorking();
            Dictionary<string, object> paramDict = this.ParameterDictionary();
            this.recordCopy = new UPRecordCopy(this.ActionConfig, this);
            this.recordCopy.StartWithSourceRecordIdentification(this.LinkRecordIdentification, paramDict);
        }

        public override UPMGroup ApplyContext(Dictionary<string, object> contextDictionary)
        {
            if (this.triggerActionGroup == null)
            {
                this.triggerActionGroup = new UPMTriggerActionGroup(this._identifier);
                string executeText = LocalizationKeys.upTextProcessExecute;
                string descriptionText = String.Empty;
                string headText = LocalizationKeys.upTextProcessAction;
                string executingText = LocalizationKeys.upTextProcessExecuting;
                string doneText = LocalizationKeys.upTextProcessDone;
                string retryText = LocalizationKeys.upTextRetry;
                if (!string.IsNullOrEmpty(this.FormItem.Label))
                {
                    var textParts = this.FormItem.Label.Split(';');
                    if (textParts.Length > 2)
                    {
                        string part0 = textParts[0];
                        if (part0.Length > 0)
                        {
                            headText = part0;
                        }

                        string part1 = textParts[1];
                        if (part1.Length > 0)
                        {
                            executeText = part1;
                        }

                        if (textParts.Length > 3)
                        {
                            string part2 = textParts[2];
                            if (part2.Length > 0)
                            {
                                executingText = part2;
                            }

                            if (textParts.Length > 4)
                            {
                                string part3 = textParts[3];
                                if (part3.Length > 0)
                                {
                                    doneText = part3;
                                }

                                descriptionText = this.FormItem.Label.Substring(part0.Length + part1.Length + part2.Length + part3.Length + 4);
                            }
                            else
                            {
                                descriptionText = this.FormItem.Label.Substring(part0.Length + part1.Length + part2.Length + 3);
                            }

                        }
                        else
                        {
                            descriptionText = this.FormItem.Label.Substring(part0.Length + part1.Length + 2);
                        }

                    }
                    else if (textParts.Length > 1)
                    {
                        string part = textParts[0];
                        if (part.Length > 0)
                        {
                            executeText = part;
                        }

                        descriptionText = this.FormItem.Label.Substring(part.Length + 1);
                    }
                    else
                    {
                        descriptionText = this.FormItem.Label;
                    }

                }

                this.triggerActionGroup.ExecutionNote = descriptionText;
                this.triggerActionGroup.LabelText = headText;
                this.triggerActionGroup.HtmlMode = false;
                this.triggerActionGroup.ButtonText = executeText;
                this.triggerActionGroup.ExecutingText = executingText;
                this.triggerActionGroup.DoneText = doneText;
                this.triggerActionGroup.RetryText = retryText;
                UPMAction menuAction = new UPMAction(this._identifier);
                this.LinkRecordIdentification = this.FormItem.ViewReference.ContextValueForKey("Func1");
                string v = contextDictionary.ValueOrDefault(this.LinkRecordIdentification) as string;
                if (!string.IsNullOrEmpty(v))
                {
                    this.LinkRecordIdentification = v;
                }

                this.ActionType = this.FormItem.ViewReference.ContextValueForKey("Func2");
                this.ActionConfig = this.FormItem.ViewReference.ContextValueForKey("Func3");
                if (this.ActionType == "CopyRecord")
                {
                    menuAction.SetTargetAction(this, PerformCopyRecordAction);
                }
                else if (this.ActionType == "Workflow")
                {
                    menuAction.SetTargetAction(this, PerformWorkflowAction);
                }

                menuAction.LabelText = executeText;
                UPMLinkRecordField field = new UPMLinkRecordField(this._identifier, menuAction);
                field.LabelText = executeText;
                field.StringValue = executeText;
                this.triggerActionGroup.AddField(field);
                this.ControllerState = GroupModelControllerState.Finished;
                this.Group = this.triggerActionGroup;
                this.AddDependingKeysFromViewReference(this.FormItem.ViewReference);
            }
            /*
            else if (Thread.IsMainThread() == false)
            {
                this.PerformSelectorOnMainThreadWithObjectWaitUntilDone(@selector(applyContext:), contextDictionary, true);
                return this.triggerActionGroup;
            }
            */

            Dictionary<string,object> paramDict = this.ParameterDictionary();
            this.triggerActionGroup.Disable = paramDict[".empty"] != null ? true : false;
            return this.triggerActionGroup;
        }

        void ShowWorking()
        {
            this.ignoreAction = true;
            this.triggerActionGroup.ErrorText = null;
            this.triggerActionGroup.Executing = true;
        }

        void ShowFinished(Exception error)
        {
            this.ignoreAction = false;
            if (error != null)
            {
                this.triggerActionGroup.ErrorText = error.Message;
            }
            else if (this.FormItem.Options.ContainsKey("multiple"))
            {
                this.triggerActionGroup.Disable = false;
                this.triggerActionGroup.Executing = false;
            }
            else
            {
                if (this.ResultMessages.Count > 0 && Convert.ToInt32(this.FormItem.Options.ValueOrDefault("HideMessage")) == 0)
                {
                    if (this.ResultMessages.Count > 1 && Convert.ToInt32(this.FormItem.Options.ValueOrDefault("ShowAllMessages")) != 0)
                    {
                        string str = string.Empty;
                        foreach (string _s in this.ResultMessages)
                        {
                            if (_s.Length < 1)
                            {
                                continue;
                            }

                            if (str.Length > 0)
                            {
                                str = str + "; " + _s; // TODO
                            }
                            else
                            {
                                str = _s;
                            }

                        }
                        this.triggerActionGroup.ExecutionNote = str;
                    }
                    else
                    {
                        this.triggerActionGroup.ExecutionNote = Convert.ToString(this.ResultMessages[0]);
                    }

                    this.triggerActionGroup.ErrorText = null;
                }

                this.triggerActionGroup.Done = true;
            }

            if (error == null)
            {
                string finishedAction = this.FormItem.Options.ValueOrDefault("FinishedAction") as string;
                if (!string.IsNullOrEmpty(finishedAction))
                {
                    Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName(finishedAction);
                    ViewReference viewReference = menu.ViewReference.ViewReferenceWith(this.outputValue);
                    this.Delegate.PerformOrganizerAction(this, viewReference);
                }

            }

        }

        public override string Value => this.outputValue;

        public void RecordCopyDidFinishWithResult(UPRecordCopy _recordCopy, List<UPCRMRecord> records)
        {
            this.recordCopy = null;
            this.offlineRequest = new UPOfflineEditRecordRequest((int)UPOfflineRequestMode.OnlineConfirm);  // TODO : removed "" from second parameter
            this.offlineRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, records, null, this);
        }

        public void RecordCopyDidFailWithError(UPRecordCopy _recordCopy, Exception error)
        {
            this.recordCopy = null;
            this.ShowFinished(error);
        }

        public void OfflineRequestDataOnlineContextDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context, object result)
        {
            this.offlineRequest = null;
            List<UPCRMRecord> copiedRecords = data as List<UPCRMRecord>;
            if (copiedRecords?.Count > 0)
            {
                UPCRMRecord record = copiedRecords[0];
                this.outputValue = record.RecordIdentification;
                this.Delegate.GroupModelControllerValueChanged(this, this.outputValue);
            }

            this.ShowFinished(null);
        }

        public void OfflineRequestDataContextDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            this.offlineRequest = null;
            this.ShowFinished(error);
        }

        void ExecuteWorkflowRequestDidFinishWithResult(UPExecuteWorkflowServerOperation sender, UPExecuteWorkflowResult result)
        {
            if (result.ChangedRecords.Count > 0)
            {
                string outputInfoAreaId = this.FormItem.Options.ValueOrDefault("OutputInfoAreaId") as string;
                int outputMultiple = Convert.ToInt32(this.FormItem.Options.ValueOrDefault("OutputMultiple"));
                this.outputValue = string.Empty;
                if (outputMultiple > 0)
                {
                    if (!string.IsNullOrEmpty(outputInfoAreaId))
                    {
                        foreach (string r in result.ChangedRecords)
                        {
                            if (r.InfoAreaId() == outputInfoAreaId)
                            {
                                if (string.IsNullOrEmpty(this.outputValue))
                                {
                                    this.outputValue = r;
                                }
                                else
                                {
                                    this.outputValue = this.outputValue + "," + r;
                                }
                            }
                        }
                    }
                    else
                    {
                        this.outputValue = String.Join(",", result.ChangedRecords.ToArray());
                    }

                }
                else if (!string.IsNullOrEmpty(outputInfoAreaId))
                {
                    foreach (string r in result.ChangedRecords)
                    {
                        if (r.InfoAreaId() == outputInfoAreaId)
                        {
                            this.outputValue = r;
                            break;
                        }

                    }
                }
                else
                {
                    this.outputValue = result.ChangedRecords[0];
                }

            }
            else
            {
                this.outputValue = string.Empty;
            }

            this.ResultMessages = result.Messages;
            this.Delegate.GroupModelControllerValueChanged(this, this.outputValue);
            this.ShowFinished(null);
        }

        void ExecuteWorkflowRequestDidFailWithError(UPExecuteWorkflowServerOperation sender, Exception error)
        {
            this.ShowFinished(error);
        }

      
        void UPExecuteWorkflowRequestDelegate.ExecuteWorkflowRequestDidFailWithError(UPExecuteWorkflowServerOperation sender, Exception error)
        {
            throw new NotImplementedException();
        }

        public void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context, Dictionary<string, object> result)
        {
            throw new NotImplementedException();
        }

        public void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            throw new NotImplementedException();
        }

        public void OfflineRequestDidFinishMultiRequest(UPOfflineRequest request)
        {
            throw new NotImplementedException();
        }

        void UPExecuteWorkflowRequestDelegate.ExecuteWorkflowRequestDidFinishWithResult(UPExecuteWorkflowServerOperation sender, UPExecuteWorkflowResult result)
        {
            throw new NotImplementedException();
        }
    }
}
