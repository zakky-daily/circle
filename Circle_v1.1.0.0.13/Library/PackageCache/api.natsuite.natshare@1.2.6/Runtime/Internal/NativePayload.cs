/* 
*   NatShare
*   Copyright (c) 2022 Yusuf Olokoba.
*/

namespace NatSuite.Sharing.Internal {

    using AOT;
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;
    using Unity.Collections;
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public abstract class NativePayload : ISharePayload {

        #region --Client API--
        /// <summary>
        /// Does the current platform support this payload?
        /// </summary>
        public static bool Supported => Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

        /// <summary>
        /// Add text to the payload.
        /// </summary>
        /// <param name="text">Plain text to add.</param>
        public virtual void AddText (string text) {
            if (payload != IntPtr.Zero)
                payload.AddText(text);
        }

        /// <summary>
        /// Add an image to the payload from a pixel buffer.
        /// The pixel buffer MUST have an RGBA8888 pixel layout.
        /// </summary>
        /// <param name="pixelBuffer">Pixel buffer containing image to add.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        public virtual void AddImage<T> (T[] pixelBuffer, int width, int height) where T : unmanaged {
            // Check
            if (payload == IntPtr.Zero)
                return;
            // Compress
            using var nativeArray = new NativeArray<T>(pixelBuffer, Allocator.Temp);
            var buffer = ImageConversion.EncodeNativeArrayToJPG(
                nativeArray,
                GraphicsFormat.R8G8B8A8_UNorm,
                (uint)width,
                (uint)height,
                quality: 90
            ).ToArray();
            // Add
            payload.AddImage(buffer, buffer.Length);
        }

        /// <summary>
        /// Add an image to the payload.
        /// Note that the image MUST be readable.
        /// </summary>
        /// <param name="image">Image to add.</param>
        public virtual void AddImage (Texture2D image) {
            // Check
            if (payload == IntPtr.Zero)
                return;
            if (!image.isReadable) {
                Debug.LogError("NatShare Error: Cannot add non-readable texture to payload");
                return;
            }
            // Add
            var jpegData = ImageConversion.EncodeToJPG(image); // Faster than PNG #85
            payload.AddImage(jpegData, jpegData.Length);
        }

        /// <summary>
        /// Add a media file to the payload.
        /// </summary>
        /// <param name="path">Path to media file to add.</param>
        public virtual void AddMedia (string uri) {
            if (payload != IntPtr.Zero)
                payload.AddMedia(uri);
        }

        /// <summary>
        /// Commit the payload.
        /// </summary>
        /// <returns>Whether the sharing action was successfully completed.</returns>
        public virtual Task<bool> Commit () {
            // Check
            if (payload == IntPtr.Zero)
                return Task.FromResult(false);
            // Commit
            var commitTask = new TaskCompletionSource<bool>();
            var handle = GCHandle.Alloc(commitTask, GCHandleType.Normal);
            payload.Commit(OnCompletion, (IntPtr)handle);
            return commitTask.Task;
        }
        #endregion


        #region --Operations--
        private readonly IntPtr payload;

        protected NativePayload (IntPtr payload) => this.payload = payload;

        [MonoPInvokeCallback(typeof(NatShare.CompletionHandler))]
        private static void OnCompletion (IntPtr context, bool success) {
            var handle = (GCHandle)context;
            var commitTask = handle.Target as TaskCompletionSource<bool>;
            handle.Free();
            commitTask?.SetResult(success);
        }
        #endregion
    }
}