using StressLevelZero.Props.Weapons;
using StressLevelZero.AI;
using PuppetMasta;

namespace ModThatIsNotMod
{
    public static class Extensions
    {
        /*
         * -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
         * https://i.imgur.com/X2w3dDo.png                                                       - June 15, 7:47  PM: i create this function, but don't test it
         * https://discord.com/channels/563139253542846474/724595991675797554/860366531287318529 - July 1,  8:49  PM: extraes first says the function doesn't work
         * https://discord.com/channels/563139253542846474/563139253542846477/862760485244633118 - July 8,  11:22 AM: extraes still can't get the function to work
         * https://discord.com/channels/563139253542846474/724595991675797554/863528582047727656 - July 10, 2:14  PM: extraes again confirms that the function doesn't work
         * https://discord.com/channels/563139253542846474/724595991675797554/899411675478900758 - July 17, 2:40  PM: extraes may not have actually ever tested the function
         * https://discord.com/channels/563139253542846474/724595991675797554/899417625036193842 - July 17, 3:04  PM: extraes now says the function does indeed work
         * https://discord.com/channels/563139253542846474/724595991675797554/899419024503496725 - July 17, 3:10  PM: extraes says that it did not work before, but it does now
         * https://discord.com/channels/563139253542846474/724595991675797554/899420057313083453 - July 17, 3:14  PM: extraes is given proof that the function was not modified
         * https://discord.com/channels/563139253542846474/724595991675797554/899420381557977139 - July 17, 3:15  PM: extraes admits that he was wrong, takes the L
         * 
         * I spent more time documenting extraes being smooth brained here than I did making the function
         * -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
         */
        public static void SetRpm(this Gun gun, float rpm)
        {
            gun.roundsPerMinute = rpm;
            gun.roundsPerSecond = rpm / 60f;
            gun.fireDuration = 60f / rpm;
        }

        public static void DealDamage(this AIBrain brain, float damage)
        {
            var health = brain?.behaviour?.health;
            if (health != null)
            {
                health.TakeDamage(1, new StressLevelZero.Combat.Attack()
                {
                    damage = damage
                });
            } 
        }
    }
}
