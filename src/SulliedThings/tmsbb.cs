using System;

namespace Unbound
{
    public class Tmsbb
    {
        public static void Init()
        {
            SulliedAI.Init();
            On.ScavengerGraphics.GenerateColors += moonColours;
            On.ScavengerGraphics.IndividualVariations.ctor += moonIndividual;
        }

        private static void moonIndividual(On.ScavengerGraphics.IndividualVariations.orig_ctor orig, ref ScavengerGraphics.IndividualVariations self, Scavenger scavenger)
        {
            orig(ref self, scavenger);
            if (scavenger?.abstractCreature != null &&
                scavenger.abstractCreature.ID.number == 1319192)
            {
                self.coloredEartlerTips = true;
            }
        }

        private static void moonColours(On.ScavengerGraphics.orig_GenerateColors orig, ScavengerGraphics self)
        {
            orig(self);
            if (self?.scavenger?.abstractCreature != null &&
                self.scavenger.abstractCreature.ID.number == 1319192)
            {
                NCRDebug.Log("Unbound active and BloodMoon ID called! Crafting the moon...");
                //
                Vector3 redvector = Custom.RGB2HSL(new Color(0.27f, 0.07f, 0.11f));
                HSLColor muenosRed = new HSLColor(redvector.x, redvector.y, redvector.z);
                Vector3 eyeVector = Custom.RGB2HSL(new Color(0.1f, 0.02f, 0.07f));
                HSLColor muenosBlack = new HSLColor(eyeVector.x, eyeVector.y, eyeVector.z);
                Vector3 decVector = Custom.RGB2HSL(new Color(0.44f, 0.18f, 0.28f));
                HSLColor muenosPink = new HSLColor(decVector.x, decVector.y, decVector.z);
                Vector3 pupVector = Custom.RGB2HSL(new Color(0.65f, 0.88f, 0.61f));
                HSLColor muenosGreen = new HSLColor(pupVector.x, pupVector.y, pupVector.z);
                // initiating colours


                self.bodyColor = muenosRed;
                self.headColor = muenosPink;
                self.bodyColorBlack = 0.5f;
                self.headColorBlack = 0.6f;
                self.decorationColor = muenosBlack;
                self.eyeColor = muenosGreen;
                self.bellyColor = muenosRed;
            }
        }
    }
}
