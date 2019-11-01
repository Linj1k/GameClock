using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace LGameClock
{
    public class LGameClock : Mod
    {
        public override string ID => "GameClock"; //Your mod ID (unique)
        public override string Name => "GameClock"; //You mod name
        public override string Author => "Linj"; //Your Username
        public override string Version => "1.0"; //Version

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => false;
        public override bool LoadInMenu => false;

        private GameObject _sun;
        private FsmFloat _rot;
        private Transform _transH, _transM;
        private bool guiEnabled;
        private readonly Rect _guiBox = new Rect(Screen.width / 2 - 107.5f, Screen.height / 2 - 85, 215, 170);
        public Settings ShowTime;

        // Keybinds
        public static Keybind LGameClockSettingsKey;

        public Settings TimeFormat;
        private FsmBool _playerInMenu;

        public LGameClock() {
            ShowTime = new Settings("ShowTime", "Activate/Deactivate GUI", true);
            TimeFormat = new Settings("TimeFormat", "24h or 12h", true);

            LGameClockSettingsKey = new Keybind("ClockKey", "Config GUI", KeyCode.T, KeyCode.LeftControl);
            Keybind.Add(this, LGameClockSettingsKey);
        }

        public override void OnLoad()
        {
            _sun = GameObject.Find("SUN/Pivot");
            _rot = _sun.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Rotation");

            _transH = GameObject.Find("SuomiClock/Clock/hour/NeedleHour").transform;
            _transM = GameObject.Find("SuomiClock/Clock/minute/NeedleMinute").transform;

            _playerInMenu = PlayMakerGlobals.Instance.Variables.FindFsmBool("PlayerInMenu");

            ModConsole.Print("GameClock has been loaded!");
            ModConsole.Print("[GameClock] Settings can be accessed by pressing Ctrl + T.");
        }

        public override void ModSettings()
        {
            Settings.AddCheckBox(this, ShowTime);
            Settings.AddCheckBox(this, TimeFormat);
        }

        // Called every tick
        public override void Update()
        {
            if (LGameClockSettingsKey.IsDown()) 
            {
                if (this.guiEnabled == false)
                {
                    _playerInMenu.Value = true;
                    this.guiEnabled = true;
                }
                else
                {
                    _playerInMenu.Value = false;
                    this.guiEnabled = false;
                }
            }
        }

        // Source : https://github.com/Wampa842/MySummerMods/tree/master/TwentyFourClock //
        /// Based on the Sun's rotation, returns whether it's the afternoon.
        public bool IsAfternoon => (_rot.Value > 330.0f || _rot.Value <= 150.0f);

        /// Hour in day, 0 to 12 float.
        /// </summary>
        public float Hour12F => ((360.0f - _transH.localRotation.eulerAngles.y) / 30.0f + 2.0f) % 12;
        /// <summary>
        /// Hour in day, 0 to 24 float.
        /// </summary>
        public float Hour24F => IsAfternoon ? Hour12F + 12.0f : Hour12F;
        /// Minute in hour, 0 to 60 float.
        public float MinuteF => (360.0f - _transM.localRotation.eulerAngles.y) / 6.0f;

        /// Hour in day, 0 to 11 integer.
        public int Hour12 => Mathf.FloorToInt(Hour12F);
        /// Hour in day, 0 to 23 integer.
        public int Hour24 => Mathf.FloorToInt(Hour24F);
        /// Minute in hour, 0 to 59 integer.
        public int Minute => Mathf.FloorToInt(MinuteF);
        // End //

        public override void OnGUI()
        {
            var myStyle = new GUIStyle();
            myStyle.fontSize = 20;
            myStyle.fontStyle = FontStyle.Bold;
            myStyle.normal.textColor = Color.white;

            if (this.guiEnabled) GUI.ModalWindow(888, this._guiBox, this.GuiSettingsWindow, "Time Format");

            if ((bool)ShowTime.GetValue())
            {
                if((bool)TimeFormat.GetValue()) {
                    GUI.Label(new Rect(Screen.width - 75, 10, 50, 20), string.Format("{0:0}h{1:00}", this.Hour24, this.Minute), myStyle);
                } else
                {
                    GUI.Label(new Rect(Screen.width - 75, 10, 50, 20), string.Format("{0:0}h{1:00}", this.Hour12, this.Minute), myStyle);
                }
            }
        }

        private void GuiSettingsWindow(int id)
        {
            if (GUI.Button(new Rect(5, 30, 100, 30), "24-Hour Clock"))
            {
                TimeFormat.Value = true;
            }
            if (GUI.Button(new Rect(110, 30, 100, 30), "12-Hour Clock"))
            {
                TimeFormat.Value = false;
            }
            if (GUI.Button(new Rect(5, 120, 100, 30), "Close"))
            {
                _playerInMenu.Value = false;
                this.guiEnabled = false;
            }
            if ((bool)ShowTime.GetValue() == true) if (GUI.Button(new Rect(110, 120, 100, 30), "Disable")) ShowTime.Value = false;
            if ((bool)ShowTime.GetValue() == false) if (GUI.Button(new Rect(110, 120, 100, 30), "Activate")) ShowTime.Value = true;
        }
    }
}
