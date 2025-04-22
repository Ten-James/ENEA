using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

public class RenderModeInteractiveServer : RenderModeAttribute
{
    public override IComponentRenderMode Mode => RenderMode.InteractiveServer;
}