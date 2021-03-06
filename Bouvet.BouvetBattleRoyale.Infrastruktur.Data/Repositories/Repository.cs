namespace Bouvet.BouvetBattleRoyale.Infrastruktur.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bouvet.BouvetBattleRoyale.Domene.Entiteter;
    using Bouvet.BouvetBattleRoyale.Infrastruktur.Data.Interfaces;
    using Bouvet.BouvetBattleRoyale.Tjenester.Interfaces;

    using log4net;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    
    public abstract class Repository<T> : IRepository<T> where T : BaseDocument
    {
        private const int RequestLimitKb = 256;

        public abstract string CollectionId { get; }

        protected readonly IKonfigurasjon _konfigurasjon;

        protected readonly IDocumentDbContext Context;

        private readonly ILog _log;
        
        private DocumentCollection _collection;

        public DocumentCollection Collection
        {
            get
            {
                if (_collection == null)
                {
                    _collection = Context.ReadOrCreateCollection(Context.Database.SelfLink, CollectionId);
                }

                return _collection;
            }
        }

        protected Repository(IKonfigurasjon konfigurasjon, IDocumentDbContext context, ILog log)
        {
            _konfigurasjon = konfigurasjon;
            Context = context;
            _log = log;
        }

        public async Task<string> Opprett(T document)
        {
            var opprettetDocument = await Context.Client.CreateDocumentAsync(Collection.SelfLink, document);
            
            return opprettetDocument.Resource.Id;
        }

        public IEnumerable<T> HentAlle()
        {
            var documents = Context.Client.CreateDocumentQuery<T>(Collection.DocumentsLink).AsEnumerable();
            
            return documents;
        }

        public T Hent(string id)
        {
            _log.Debug("Henter " + id);

            var document = Context.Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                .Where(d => d.DocumentId == id)
                .AsEnumerable()
                .FirstOrDefault();

            return document;
        }

        public async Task Oppdater(T document)
        {
            var oppdaterStart = DateTime.Now;

            var options = new RequestOptions
                              {
                                  AccessCondition = new AccessCondition
                                                        {
                                                            Type = AccessConditionType.IfMatch, 
                                                            Condition = document.Etag
                                                        }
                              };

            try
            {
                await Context.Client.ReplaceDocumentAsync(document.SelfLink, document, options);
            }
            catch (DocumentClientException documentClientException)
            {
                _log.Warn("DocumentClientException ble fanget i Oppdater() p� document " + document.DocumentId);

                throw new Exception("Oppdatering ble fors�kt p� utdatert objekt, pr�v igjen.", documentClientException);
            }

            var oppdaterEnd = DateTime.Now;

            LoggOppdatering(document, oppdaterStart, oppdaterEnd);
        }

        public async Task Slett(T document)
        {
            var slettStart = DateTime.Now;

            await Context.Client.DeleteDocumentAsync(document.SelfLink, new RequestOptions());

            var slettEnd = DateTime.Now;

            LoggSletting(document, slettStart, slettEnd);
        }

        public IEnumerable<T> S�k(Func<T, bool> predicate)
        {
            var documents = Context.Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                    .Where(predicate)
                    .AsEnumerable();

            return documents;
        }

        private void LoggOppdatering(T document, DateTime oppdaterStart, DateTime oppdaterEnd)
        {
            var documentStorrelse = EnhetConverter.HentObjektStorrelse(document);

            var deltaTid = oppdaterStart.Subtract(oppdaterEnd);

            string loggMelding = "Oppdatering av " + document.DocumentId + " p� " + documentStorrelse + "kb tok..."
                                 + deltaTid.TotalSeconds + " sekunder";

            var oppdateringTidSomSekunder = deltaTid.Duration().TotalSeconds;

            if (oppdateringTidSomSekunder > 5)
                _log.Warn("Treg oppdatering, tok " + oppdateringTidSomSekunder);

            if (documentStorrelse > RequestLimitKb)
            {
                _log.Warn(loggMelding);
            }
            else
            {
                _log.Debug(loggMelding);
            }
        }

        private void LoggSletting(T document, DateTime slettStart, DateTime slettEnd)
        {
            var documentStorrelse = EnhetConverter.HentObjektStorrelse(document);

            var loggMelding = "Sletting av " + document.DocumentId + " p� " + document + "kb tok..."
                              + slettStart.Subtract(slettEnd).TotalSeconds + " sekunder";

            if (documentStorrelse > RequestLimitKb)
            {
                _log.Warn(loggMelding);
            }
            else
            {
                _log.Debug(loggMelding);
            }
        }
    }
}