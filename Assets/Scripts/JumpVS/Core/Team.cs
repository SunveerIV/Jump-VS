namespace JumpVS.Core {
    public struct Team {
        public byte Id { get; private set; }

        public Team(byte Id) {
            this.Id = Id;
        }
    }
}