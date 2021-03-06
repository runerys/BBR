﻿namespace Bouvet.BouvetBattleRoyale.Tjenester.Interfaces
{
    using Bouvet.BouvetBattleRoyale.Domene;
    using Bouvet.BouvetBattleRoyale.Domene.Entiteter;

    public interface IPoengService
    {
        //Straffemetoder
        Lag SjekkOgSettPifPingStraff(Lag lag);
        Lag SjekkOgSettInfisertSoneStraff(Lag lag);

        Lag SettFritekstMeldingSendtStraff(Lag lag, Melding melding);

        //Poengtildeling
        Lag SettPoengForKodeRegistrert(Lag lag, HendelseType hendelse, int postnummer);

        Lag SettPoengForLag(Lag lag, int poeng, string kommentar);
    }
}
