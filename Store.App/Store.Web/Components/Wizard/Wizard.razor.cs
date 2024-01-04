using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Store.Shared.Dto;
using System;

namespace Store.Web.Components.Wizard
{
    public partial class Wizard
    {
        [Parameter]
        public EventCallback OnSubmit { get; set; }

        [Parameter]
        public bool IsVisible { get; set; } = false;

        protected internal List<WizardStep> Steps = new List<WizardStep>();

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public WizardStep ActiveStep { get; set; }

        [Parameter]
        public int ActiveStepIx { get; set; }

        public bool IsLastStep { get; set; }

        protected internal void GoBack()
        {
            if (ActiveStepIx > 0)
                SetActive(Steps[ActiveStepIx - 1]);
        }

        protected internal void GoNext()
        {
            if (ActiveStep.IsStepValid)
            {
                if (ActiveStepIx < Steps.Count - 1)
                {
                    SetActive(Steps[(Steps.IndexOf(ActiveStep) + 1)]);
                }
                else
                {
                    OnSubmit.InvokeAsync();
                }
            }
        }

        protected internal void SetActive(WizardStep step, bool isfirstRender = false)
        {
            ActiveStep = step ?? throw new ArgumentNullException(nameof(step));

            if (ActiveStep != null && ActiveStep.IsStepValid)
            {

                ActiveStepIx = StepsIndex(step);
                if (ActiveStepIx == Steps.Count - 1)
                    IsLastStep = true;
                else
                    IsLastStep = false;
            }
        }

        public int StepsIndex(WizardStep step) => StepsIndexInternal(step);

        protected int StepsIndexInternal(WizardStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            return Steps.IndexOf(step);
        }

        protected internal void AddStep(WizardStep step)
        {
            Steps.Add(step);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                SetActive(Steps[0], isfirstRender: firstRender);
                StateHasChanged();
            }
        }

        protected override void OnParametersSet()
        {
            StateHasChanged();
        }

        private void Close()
        {
            IsVisible = false;
        }
    }
}
