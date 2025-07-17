using System.Diagnostics;

namespace LearningHub.Nhs.WebUI.BlazorComponents.E2ETests.BlazeWright
{
    public static class BlazorPageExtensions
    {
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static Task<IResponse?> GotoOnceNetworkIsIdleAsync(this IPage page, string url)//GotoPreRenderedAsync
            => page.GotoAsync(url, new() { WaitUntil = WaitUntilState.NetworkIdle });//this is by default waiting until prerendering is over qqqq but mvc it should still work but check?
    }
}
