namespace MyGame
{
    public class AudioMixer
    {
        private int lightningHitChannel = 0;
        private int shootChannel = 1;
        private int walkChannel = 2;
        private int jumpChannel = 3;
        private int hitChannel = 4;
        private int anvilFallChannel = 5;
        private int anvilHitChannel = 6;
        private int landChannel = 7;
        private int teleportChannel = 8;
        private int hitEnemyChannel = 9;
        private int uiChannel = 10;
        private int musicChannel = 11;
        private int uiClickChannel = 12;
        private int uiDifficulty = 13;
        private int lightningShootChannel = 14;
        private int bubblePopChannel = 15;
        private int bubbleShootChannel = 16;
        private int lightningCastChannel = 17;

        public int LightningHitChannel
        {
            get { return lightningHitChannel; }
            private set { lightningHitChannel = value; }
        }

        public int ShootChannel
        {
            get { return shootChannel; }
            private set { shootChannel = value; }
        }

        public int WalkChannel
        {
            get { return walkChannel; }
            private set { walkChannel = value; }
        }

        public int JumpChannel
        {
            get { return jumpChannel; }
            private set { jumpChannel = value; }
        }

        public int HitChannel
        {
            get { return hitChannel; }
            private set { hitChannel = value; }
        }

        public int AnvilFallChannel
        {
            get { return anvilFallChannel; }
            private set { anvilFallChannel = value; }
        }

        public int AnvilHitChannel
        {
            get { return anvilHitChannel; }
            private set { anvilHitChannel = value; }
        }

        public int LandChannel
        {
            get { return landChannel; }
            private set { landChannel = value; }
        }

        public int TeleportChannel
        {
            get { return teleportChannel; }
            private set { teleportChannel = value; }
        }

        public int HitEnemyChannel
        {
            get { return hitEnemyChannel; }
            set { hitEnemyChannel = value; }
        }

        public int UIChannel
        {
            get { return uiChannel; }
            set { uiChannel = value; }
        }

        public int MusicChannel
        {
            get { return musicChannel; }
            set { musicChannel = value; }
        }

        public int UIClickChannel
        {
            get { return uiClickChannel; }
            set { uiClickChannel = value; }
        }

        public int UIDifficulty
        {
            get { return uiDifficulty; }
            set { uiDifficulty = value; }
        }

        public int LightningShootChannel
        {
            get { return lightningShootChannel; }
            set { lightningShootChannel = value; }
        }

        public int BubblePopChannel
        {
            get { return bubblePopChannel; }
            set { bubblePopChannel = value; }
        }

        public int BubbleShootChannel
        {
            get { return bubbleShootChannel; }
            set { bubbleShootChannel = value; }
        }

        public int LightningCastChannel
        {
            get { return lightningCastChannel; }
            set { lightningCastChannel = value; }
        }
    }
}
