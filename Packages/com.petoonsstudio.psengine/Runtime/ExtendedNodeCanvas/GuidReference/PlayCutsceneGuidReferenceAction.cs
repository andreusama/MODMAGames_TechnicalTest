using ParadoxNotion.Design;
using PetoonsStudio.PSEngine.Timeline;

namespace PetoonsStudio.PSEngine.NodeCanvas
{
    [Name("Play Cutscene")]
    [Description("Plays and waits the cutscene.")]
    public class PlayCutsceneGuidReferenceAction : GuidReferenceActionTask<Cutscene>
    {
        protected override void OnExecute()
        {
            base.OnExecute();

            if(m_Target.Play())
            {
                m_Target.OnCutsceneStop.AddListener(CutsceneStopped);
            }
            else
            {
                EndAction(false);
            }
        }

        protected void CutsceneStopped()
        {
            m_Target.OnCutsceneStop.RemoveListener(CutsceneStopped);

            EndAction(true);
        }
    }
}
