using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Assets.Source.Dialog
{
    [AddComponentMenu("RQ/Components/Set Dialog Database")]
    public class SetDialogComponent : MonoBehaviour
    {
        [SerializeField]
        private DialogueDatabase _dialogueDatabase;

        public void Start()
        {
            DialogueManager.AddDatabase(_dialogueDatabase);
        }

        //public void SetDialogueDatabase(DialogueDatabase dialogueDatabase)
        //{
        //    // Turning this off for now, it is slow.
        //    //return;
        //    //DialogManager.RemoveDatabase
        //    // If the database is changing, remove the old one
        //    if (_dialogueDatabase != null && _dialogueDatabase != dialogueDatabase)
        //    {
        //        Log.Info("Removing database " + _dialogueDatabase.name);
        //        //Debug.Log("Removing database " + _dialogueDatabase.name);
        //        DialogueManager.RemoveDatabase(_dialogueDatabase);
        //    }

        //    // If the database is null or it is changing, add the new one
        //    if (_dialogueDatabase == null || _dialogueDatabase != dialogueDatabase)
        //    {
        //        _dialogueDatabase = dialogueDatabase;
        //        if (_dialogueDatabase == null)
        //            return;
        //        //if (_dialogueDatabase != null)
        //        //{
        //        Log.Info("Adding database " + _dialogueDatabase.name);
        //        //Debug.Log("Adding database " + _dialogueDatabase.name);
        //        DialogueManager.AddDatabase(_dialogueDatabase);
        //    }
        //    //DialogueManager.Instance.PreloadResources();
        //    //DialogueManager.Instance.PreloadMasterDatabase();
        //    //_dialogManager.initialDatabase = dialogueDatabase;
        //    //_dialogManager.PreloadResources();
        //    //.
        //    //PersistentDataManager.Apply();
        //    //DialogueManager.Instance.PreloadResources();
        //}
    }
}
