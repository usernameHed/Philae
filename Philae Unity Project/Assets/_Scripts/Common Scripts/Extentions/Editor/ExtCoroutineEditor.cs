using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;


namespace hedCommon.extension.editor
{
    public static class ExtCoroutineEditor
    {
        public delegate void ActionToExecute();
        public delegate void ActionToExecute<T>(T parameter);

        /// <summary>
        /// use: ExtCoroutineEditor.ExecuteActionAfterSeconds(FocusAfterOneFrame, 0.1f, this);
        /// </summary>
        /// <param name="editor"></param>
        public static void ExecuteActionAfterSeconds(ActionToExecute action, float seconds, object editor)
        {
            EditorCoroutineUtility.StartCoroutine(WaitForSeconds(action, seconds), editor);
        }

        private static IEnumerator WaitForSeconds(ActionToExecute action, float seconds)
        {
            yield return new EditorWaitForSeconds(seconds);
            action();
        }

        /// <summary>
        /// use: ExtCoroutineEditor.ExecuteActionAfterSeconds(FocusAfterOneFrame, 0.1f, this);
        /// </summary>
        /// <param name="editor"></param>
        public static void ExecuteActionAfterSeconds<T>(ActionToExecute<T> action, T param, float seconds, object editor)
        {
            EditorCoroutineUtility.StartCoroutine(WaitForSeconds(action, param, seconds), editor);
        }

        private static IEnumerator WaitForSeconds<T>(ActionToExecute<T> action, T param, float seconds)
        {
            yield return new EditorWaitForSeconds(seconds);
            action(param);
        }

        /// <summary>
        /// use: ExtCoroutineEditor.ExecuteActionAfterFrames(FocusAfterOneFrame, 1, this);
        /// ///private static void FocusAfterOneFrame<T>(T item) where T : RCamera
        /// {
        ///    ExtSceneView.FocusOnSelection(item.GetPositionFollow(), 70);
        /// }
        /// </summary>
        /// <param name="editor"></param>
        public static void ExecuteActionAfterFrames(ActionToExecute action, int frameBeforeExecuteAction, object editor)
        {
            EditorCoroutineUtility.StartCoroutine(WaitForFrames(action, frameBeforeExecuteAction), editor);
        }

        private static IEnumerator WaitForFrames(ActionToExecute action, int frameBeforeExecuteAction)
        {
            for (int i = 0; i < frameBeforeExecuteAction; i++)
            {
                yield return null;
            }
            action();
        }

        /// <summary>
        /// use: ExtCoroutineEditor.ExecuteActionAfterFrames(FocusAfterOneFrame, item, 1, this);
        /// 
        ///private static void FocusAfterOneFrame<T>(T item) where T : RCamera
        ///{
        ///    ExtSceneView.FocusOnSelection(item.GetPositionFollow(), 70);
        ///}
        /// </summary>
        /// <param name="editor"></param>
        public static void ExecuteActionAfterFrames<T>(ActionToExecute<T> action, T param, int frameBeforeExecuteAction, object editor)
            {
                EditorCoroutineUtility.StartCoroutine(WaitForFrames(action, param, frameBeforeExecuteAction), editor);
            }

            //EditorCoroutineUtility.StartCoroutine(CountEditorUpdates(), this);

            private static IEnumerator WaitForFrames<T>(ActionToExecute<T> action, T item, int frameBeforeExecuteAction)
            {
                for (int i = 0; i < frameBeforeExecuteAction; i++)
                {
                    yield return null;
                }
                action(item);
            }
    }
}