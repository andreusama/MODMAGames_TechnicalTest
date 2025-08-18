#if UNITY_SWITCH
using nn.hid;
using nn.account;
using nn.util;
using nn.fs;
#endif
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System;
using System.Threading.Tasks;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;

namespace PetoonsStudio.PSEngine.Multiplatform.Switch
{
    public class SwitchManager : PersistentSingleton<SwitchManager>, IPlatformManager<SwitchConfig>
    {
#if UNITY_SWITCH
        public Uid UserId = Uid.Invalid; // user ID for the user account on the Nintendo Switch
        public NpadId NpadId;
        public NpadStyle SupportedNPadStyles { get => m_SwitchConfig.SupportedNpadStyles; }

        public bool IsSinglePlayer { get => m_SwitchConfig.isSinglePlayer; }
        public string GetMountName { get => m_SwitchConfig.MountName; }

        private bool m_AppletCD = false;
        private SwitchConfig m_SwitchConfig;
        private SwitchBoostMode m_BoostModeController;
#endif

        public async Task Initialize(SwitchConfig config)
        {
#if UNITY_SWITCH
            m_SwitchConfig = config;

            InitializeUser(ref UserId);
            MountDisk();
            InitializeInternalService(config);

            //Show Applet on start
            if (m_SwitchConfig.ShowAppletOnStart)
            {
                ShowApplet();
            }

            await Task.CompletedTask;
#else
            await Task.Yield();
#endif
        }

#if UNITY_SWITCH
        private void InitializeInternalService(SwitchConfig config)
        {
            PlatformServices switchServices = new PlatformServices();

            switchServices.Storage = PlatformManager.CreateStorageService(config, new SwitchFileStorage());
            switchServices.DownloadableContentFinder = new SwitchDownloadableContentFinder();

            PlatformManager.Instance.SetPlatformServices(switchServices);

            if (config.AllowCPUBoostMode)
            {
                m_BoostModeController = this.gameObject.AddComponent<SwitchBoostMode>();
                m_BoostModeController.Initialize(config.MaxTimeBoostMode);
            }
        }



        /// <summary>
        /// Show Controller Applet
        /// </summary>
        public void ShowControllerApplet()
        {
            Npad.Initialize();

            // Set supported styles
            // FullKey = Pro Controller
            // JoyDual = Two Joy-Con used as one controller
            // see nn::hid::SetSupportedNpadStyleSet (NpadStyleSet style) in the SDK docs for API details
            Npad.SetSupportedStyleSet(SupportedNPadStyles);

            // NpadJoy.SetHoldType only affects how controllers behave when using a system applet.
            // NpadJoyHoldType.Vertical is the default setting, but most games would want to use NpadJoyHoldType.Horizontal as it can support all controller styles
            // If you use NpadJoyHoldType.Vertical,  Npad.SetSupportedStyleSet must only list controller types supported by NpadJoyHoldType.Vertical.
            // If you don't, the controller applet will fail to load.
            // Supported types for NpadJoyHoldType.Vertical are: NpadStyle.JoyLeft, NpadStyle.JoyRight
            NpadJoy.SetHoldType(NpadJoyHoldType.Horizontal);

            NpadJoy.SetHandheldActivationMode(NpadHandheldActivationMode.Dual); //both controllers must be docked for handheld mode.

            //You must call Npad.SetSupportedIdType for all supported controllers.
            //Your game may run if you don't call this, but not calling this may lead to crashes in some circumstances.
            NpadId[] npadIds = { NpadId.Handheld, NpadId.No1, NpadId.No2, NpadId.No3, NpadId.No4 };
            Npad.SetSupportedIdType(npadIds);

            // set the arguments for the applet
            // see nn::hid::ControllerSupportArg::SetDefault () in the SDK documentation for details
            ControllerSupportArg controllerSupportArgs = new ControllerSupportArg();
            controllerSupportArgs.SetDefault();
            controllerSupportArgs.enableSingleMode = IsSinglePlayer;

            UnityEngine.Switch.Applet.Begin();

            ControllerSupportResultInfo result = new ControllerSupportResultInfo();

            ControllerSupport.Show(ref result, controllerSupportArgs);


            UnityEngine.Switch.Applet.End();

            NpadId = result.selectedId;
        }

        /// <summary>
        /// Show Controller Applet with an specific number of NpadIDs
        /// </summary>
        public void ShowControllerApplet(NpadId[] npadIds, byte numPlayers)
        {
            Npad.Initialize();

            // Set supported styles
            // FullKey = Pro Controller
            // JoyDual = Two Joy-Con used as one controller
            // see nn::hid::SetSupportedNpadStyleSet (NpadStyleSet style) in the SDK docs for API details
            Npad.SetSupportedStyleSet(SupportedNPadStyles);

            // NpadJoy.SetHoldType only affects how controllers behave when using a system applet.
            // NpadJoyHoldType.Vertical is the default setting, but most games would want to use NpadJoyHoldType.Horizontal as it can support all controller styles
            // If you use NpadJoyHoldType.Vertical,  Npad.SetSupportedStyleSet must only list controller types supported by NpadJoyHoldType.Vertical.
            // If you don't, the controller applet will fail to load.
            // Supported types for NpadJoyHoldType.Vertical are: NpadStyle.JoyLeft, NpadStyle.JoyRight
            NpadJoy.SetHoldType(NpadJoyHoldType.Horizontal);

            NpadJoy.SetHandheldActivationMode(NpadHandheldActivationMode.Dual); //both controllers must be docked for handheld mode.

            //You must call Npad.SetSupportedIdType for all supported controllers.
            //Your game may run if you don't call this, but not calling this may lead to crashes in some circumstances.
            Npad.SetSupportedIdType(npadIds);

            // set the arguments for the applet
            // see nn::hid::ControllerSupportArg::SetDefault () in the SDK documentation for details
            ControllerSupportArg controllerSupportArgs = new ControllerSupportArg();
            controllerSupportArgs.SetDefault();
            controllerSupportArgs.playerCountMax = numPlayers;
            controllerSupportArgs.enableSingleMode = IsSinglePlayer;

            UnityEngine.Switch.Applet.Begin();

            ControllerSupportResultInfo result = new ControllerSupportResultInfo();

            ControllerSupport.Show(ref result, controllerSupportArgs);

            UnityEngine.Switch.Applet.End();

            NpadId = result.selectedId;
        }

        public NpadStyle GetStyle(NpadId id)
        {
            return Npad.GetStyleSet(id);
        }

        public void ShowApplet()
        {
            if (!m_AppletCD)
            {
                ShowControllerApplet();
                StartCoroutine(AppletCD());
            }
        }

        /// <summary>
        /// Show Controller Applet with an specific number of NpadIDs
        /// </summary>
        public void ShowApplet(NpadId[] npadIds, byte numPlayers)
        {
            if (!m_AppletCD)
            {
                ShowControllerApplet(npadIds, numPlayers);
                StartCoroutine(AppletCD());
            }
        }

        /// <summary>
        /// Initialize Current user of Switch and save it in uid
        /// </summary>
        /// <param name="uid"></param>
        private void InitializeUser(ref Uid uid)
        {
            Account.Initialize();

            // Open the user that was selected before the application started.
            // This assumes that Startup user account is set to Required.
            UserHandle userHandle = new UserHandle();
            Account.TryOpenPreselectedUser(ref userHandle);

            // Get the user ID of the preselected user account.
            Account.GetUserId(ref uid, userHandle);
        }

        /// <summary>
        /// Mount save data archive based on the user
        /// </summary>
        private void MountDisk()
        {
            // Mount the save data archive as "save" for the selected user account.
            nn.Result result = SaveData.Mount(GetMountName, UserId);
        }

        private Color4u8 ConvertColor(Color color)
        {
            Color4u8 switchColor = new Color4u8();
            switchColor.Set((byte)color.r, (byte)color.g, (byte)color.b, (byte)color.a);
            return switchColor;
        }

        private IEnumerator AppletCD()
        {
            m_AppletCD = true;
            yield return new WaitForSecondsRealtime(m_SwitchConfig.AppletCooldown);
            m_AppletCD = false;
        }
#endif
    }
}
