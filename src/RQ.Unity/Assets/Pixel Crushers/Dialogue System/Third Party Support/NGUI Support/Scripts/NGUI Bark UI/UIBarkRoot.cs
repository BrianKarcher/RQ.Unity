using UnityEngine;

namespace PixelCrushers.DialogueSystem.NGUISupport
{

    /// <summary>
    /// To use NGUI barks, you must add one copy of this component to a game object in an NGUI UI.
    /// All barking NPCs will add their NGUI labels as children of this object.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/NGUI/NGUI Bark Root")]
    public class UIBarkRoot : MonoBehaviour
    {

        public static GameObject rootObject = null;

        public bool dontDestroyUIRootOnLoad = false;

        public void Awake()
        {
            rootObject = gameObject;
            if (dontDestroyUIRootOnLoad)
            {
                Transform t = transform;
                UIRoot uiRoot = null;
                while ((uiRoot == null) && (t != null))
                {
                    uiRoot = t.GetComponent<UIRoot>();
                    t = t.parent;
                }
                if (uiRoot != null) DontDestroyOnLoad(uiRoot.gameObject);
            }
        }

    }

}
