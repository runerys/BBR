Bouvet Battle Royale
====================

Bouvet Battle Royale er en spillplattform som tilbyr et API til et slags orienteringsløp hvor man samler poster og koder underveis.

# Arkitektur #

## Domenemodell ##

- TODO: link til gliffy

## Arkiveringsstrategi med skalering ##

- TODO: link til gliffy

# Teknisk oppsett #

## Hosting i LINQPad ##

Åpne skriptet HostingILINQPad.linq og kjør skriptet. For å fjerne log4net-advarsel om at LINQPad ikke finner log4net-konfig så gå til C:\Program Files (x86)\LINQPad4 og opprett en LINQPad.config-fil med log4net-konfigen fra Owin-prosjektets App.config.

## Integrasjonstester ##

### API-tester ###

Integrasjonstestene starter opp en OWIN-applikasjon mot [http://localhost:52501](http://localhost:52501) i `BaseApiTest`.
Testene bruker API'et til å sette opp data/tilstand før selve testen kjøres. Data/tilstand blir så fjernet etter testkjøring. Det brukes konsekvent en gitt LagId (Testlag1) for testing for å unngå å slette ekte data.

Testene kjører mot databasen som ligger i app.config i integrasjonstestprosjektet

### DataAksess-tester ###

Integrasjonstester mot Repositories oppretter hver sin Repository-implementasjon og kjører mot databasen definert i app.config. Før og etter hver test blir data/tilstand opprettet og slettet, se `BaseRepositoryIntegrasjonstest.cs`