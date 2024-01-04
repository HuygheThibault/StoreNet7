using Microsoft.AspNetCore.Components;

namespace Store.Web.Components.Wizard
{
    public partial class WizardStep
    {
        [CascadingParameter]
        protected internal Wizard Parent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public bool IsStepValid { get; set; } = false;

        protected override void OnInitialized()
        {
            Parent.AddStep(this);
        }
    }
}
