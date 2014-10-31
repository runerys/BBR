namespace Bouvet.BouvetBattleRoyale.Tjenester.Interfaces
{
    using Bouvet.BouvetBattleRoyale.Domene.Entiteter;

    public interface ILagGameService
    {
        Lag HentLagMedLagId(string lagId);

        PifPosisjon HentSistePifPosisjon(string lagId);
    }
}