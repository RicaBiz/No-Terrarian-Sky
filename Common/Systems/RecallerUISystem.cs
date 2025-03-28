using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace TerrarianSky.Common.Systems
{
    internal class RecallerUISystem : ModSystem
    {
        
        internal class RecallerUI : UIState
        {
            UIPanel panel;
            public override void OnInitialize()
            {
                panel = new UIPanel() {
				    HAlign = 0.5f,
				    VAlign = 0.5f,
				    Width = new(400, 0f),
				    Height = new(240, 0f)
			    };
			    Append(panel);

                var header = new UIText("Viewing Worlds") {
				    IsWrapped = true,
				    Width = StyleDimension.Fill,
				    HAlign = 0.5f
			    };
                panel.Append(header);

                var backButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f) {
				    TextColor = Color.Red,
				    VAlign = 1f,
				    Width = new(-10f, 1 / 3f),
				    Height = new(30f, 0f)
			    };
			    backButton.WithFadedMouseOver();
			    backButton.OnLeftClick += BackButton_OnLeftClick;
			    panel.Append(backButton);

                var createWorldButton = new UITextPanel<string>("Create World", 0.7f) {
				    TextColor = Color.Yellow,
				    VAlign = 1f,
                    HAlign = 1f,
				    Width = new(-10f, 1 / 3f),
				    Height = new(30f, 0f)
			    };
			    createWorldButton.WithFadedMouseOver();
			    createWorldButton.OnLeftClick += CreateWorldButton_OnLeftClick;
			    panel.Append(createWorldButton);

            }

            private void BackButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
            {
                ModContent.GetInstance<RecallerUISystem>().HideRecallerUI();
            }

            private void CreateWorldButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
            {
                WorldManagerSystem.worldManager.MakeNewWorld();
            }

            public void UpdateNames()
            {
                var worldNames = WorldManagerSystem.worldManager.CreatedWorldNames;

                float increase = 1 / 3f;
                float currentVAlign = 1 / 8f;
                foreach(var name in worldNames)
                {
                    var worldButton = new UITextPanel<string>(name, 1f)
                    {
                        TextColor = Color.AntiqueWhite,
                        HAlign = 0.5f,
                        VAlign = currentVAlign,
                        Width = StyleDimension.Fill,
                        Height = StyleDimension.FromPercent(1 / 4f),
                    };
			        worldButton.WithFadedMouseOver();
			        worldButton.OnLeftClick += WorldButtonLoad;
			        panel.Append(worldButton);
                    currentVAlign += increase;
                }
            }

            private void WorldButtonLoad(UIMouseEvent evt, UIElement listeningElement)
            {
                WorldManagerSystem.worldManager.LoadWorld(((UITextPanel<string>)listeningElement).Text);
                ModContent.GetInstance<RecallerUISystem>().HideRecallerUI();
            }

        }


        internal UserInterface userInterface;
        internal RecallerUI ui;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                userInterface = new UserInterface();
                ui = new();
                ui.Activate();
            }
        }

        public override void Unload()
        {
            ui = null;
        }

        private GameTime _lastUpdateUiGameTime;
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (userInterface?.CurrentState != null)
            {
                userInterface.Update(gameTime);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Terrarian Sky: RecallerUI",
    	            delegate
                    {
                        if ( _lastUpdateUiGameTime != null && userInterface?.CurrentState != null) {
                            userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        internal void ShowRecallerUI() {
            IngameFancyUI.OpenUIState(ui);
            ui.UpdateNames();
        }

        internal void HideRecallerUI() {
            IngameFancyUI.Close();
        }



    }
}
