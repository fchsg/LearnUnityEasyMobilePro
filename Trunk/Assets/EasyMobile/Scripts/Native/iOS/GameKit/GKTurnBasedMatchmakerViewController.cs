﻿#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.iOS.UIKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.GameKit
{
    using DelegateForwarder = EasyMobile.Internal.iOS.GameKit.GKTurnBasedMatchmakerViewControllerDelegateForwarder;

    /// <summary>
    /// A user interface that allows players to manage the turn-based matches that they are participating in.
    /// </summary>
    internal class GKTurnBasedMatchmakerViewController : UIViewController /* GKTurnBasedMatchmakerViewController actually directly inherits UINavigationController
    // rather than UIViewController. */
    {
        #region GKTurnBasedMatchmakerViewControllerDelegate

        /// <summary>
        /// Your game implements the GKTurnBasedMatchmakerViewControllerDelegate protocol on an object 
        /// to respond to events generated by a <see cref="GKTurnBasedMatchmakerViewController"/> object.
        /// </summary>
        public interface GKTurnBasedMatchmakerViewControllerDelegate
        {
            /// <summary>
            /// Called when the player cancels matchmaking.
            /// </summary>
            /// <param name="viewController">View controller.</param>
            void TurnBasedMatchmakerViewControllerWasCancelled(GKTurnBasedMatchmakerViewController viewController);

            /// <summary>
            /// Called when an error occurs.
            /// </summary>
            /// <param name="viewController">View controller.</param>
            /// <param name="error">Error.</param>
            void TurnBasedMatchmakerViewControllerDidFailWithError(GKTurnBasedMatchmakerViewController viewController, NSError error);
        }

        #endregion

        private DelegateForwarder mDelegateForwarder;
        private GKTurnBasedMatchmakerViewControllerDelegate mDelegate;

        internal GKTurnBasedMatchmakerViewController(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Initializes a new matchmaker view controller.
        /// </summary>
        /// <param name="request">Request.</param>
        public GKTurnBasedMatchmakerViewController(GKMatchRequest request)
            : this(C.GKTurnBasedMatchmakerViewController_initWithMatchRequest(request != null ? request.ToPointer() : IntPtr.Zero))
        {
            // We're using a pointer returned by a native constructor: must call CFRelease().
            CoreFoundation.CFType.CFRelease(this.ToPointer());
        }

        /// <summary>
        /// Gets or sets a value indicating whether this view controller
        /// should show existing matches.
        /// </summary>
        /// <value><c>true</c> if show existing matches; otherwise, <c>false</c>.</value>
        public bool ShowExistingMatches
        {
            get
            {
                return C.GKTurnBasedMatchmakerViewController_showExistingMatches(SelfPtr());
            }
            set
            {
                C.GKTurnBasedMatchmakerViewController_setShowExistingMatches(SelfPtr(), value);
            }
        }

        /// <summary>
        /// Gets or sets the delegate for this view controller.
        /// If you need to update the delegate methods, it is strongly recommended
        /// to change the existing delegate's actions via its properties, rather than 
        /// creating a whole new delegate object, which can be expensive. 
        /// </summary>
        /// <value>The turn based matchmaker delegate.</value>
        public GKTurnBasedMatchmakerViewControllerDelegate TurnBasedMatchmakerDelegate
        {
            get
            {
                return mDelegate;
            }
            set
            {
                mDelegate = value;

                if (mDelegate == null)
                {
                    // Nil out the native delegate.
                    mDelegateForwarder = null;
                    C.GKTurnBasedMatchmakerViewController_setTurnBasedMatchmakerDelegate(SelfPtr(), IntPtr.Zero);
                }
                else
                {
                    // Create a delegate forwarder if needed.
                    if (mDelegateForwarder == null)
                    {
                        mDelegateForwarder = InteropObjectFactory<DelegateForwarder>.Create(
                            () => new DelegateForwarder(),
                            fwd => fwd.ToPointer());

                        // Assign on native side.
                        C.GKTurnBasedMatchmakerViewController_setTurnBasedMatchmakerDelegate(SelfPtr(), mDelegateForwarder.ToPointer());
                    }

                    // Set delegate.
                    mDelegateForwarder.Listener = mDelegate;
                }
            }
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern /* InteropGKTurnBasedMatchmakerViewController */ IntPtr 
            GKTurnBasedMatchmakerViewController_initWithMatchRequest(/* InteropGKMatchRequest */ IntPtr requestPointer);

            [DllImport("__Internal")][return:MarshalAs(UnmanagedType.I1)]
            internal static extern bool GKTurnBasedMatchmakerViewController_showExistingMatches(
                /* InteropGKTurnBasedMatchmakerViewController */ HandleRef self);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatchmakerViewController_setShowExistingMatches(
                /* InteropGKTurnBasedMatchmakerViewController */ HandleRef self,
                                                                 bool showExistingMatches);

            [DllImport("__Internal")]
            internal static extern /* InteropGKTurnBasedMatchmakerViewControllerDelegate */ IntPtr 
            GKTurnBasedMatchmakerViewController_turnBasedMatchmakerDelegate(
                /* InteropGKTurnBasedMatchmakerViewController */ HandleRef self);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatchmakerViewController_setTurnBasedMatchmakerDelegate(
                /* InteropGKTurnBasedMatchmakerViewController */ HandleRef self,
                /* InteropGKTurnBasedMatchmakerViewControllerDelegate */IntPtr turnBasedMatchmakerDelegatePointer);
        }

        #endregion
    }
}
#endif
