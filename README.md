# MO-FS Prototype

Dette prosjektet er en prototype for integrasjon mellom Mobility Online (MO) og Felles studentsystem (FS). Applikasjonen leser studentdata fra en XML-eksport fra Mobility Online, henter ut relevante felter, transformerer dataene til formatet FS forventer, og sender studentene videre til FS via GraphQL. 

Merk at XML-eksporten ikke ligger i repoet/prototypen. Den må hentes ut manuelt som en eksport fra Mobility Online og legges inn lokalt før applikasjonen kjøres.

## Hovedflyt
```bash
MO XML-fil
  ↓
XML-lesing
  ↓
Parsing av relevante felt
  ↓
Transformasjon til FS-format
  ↓
Bygging av FS-payload
  ↓
GraphQL POST til FS
```

## Teknologier

- C# / .NET 8
- XML-deserialisering
- GraphQL
- HTTP POST
- Podman
- Miljøvariabler for konfigurasjon

## Prosjektstruktur
```text
src/MoFs.Prototype/
  Configuration/
    FsIntegrationOptions.cs
    appsettings.json

  Models/
    MobilityOnlineExport.cs
    MobilityOnlineRow.cs
    MobilityOnlineStudent.cs
    FsPayloadModels.cs
    GraphQlRequest.cs
    FsGraphQlOptions.cs
    FsGraphQlClient.cs

  Services/
    MobilityOnlineXmlReader.cs
    MobilityOnlineParser.cs
    FsTransformationService.cs
    FoedselsnummerService.cs
    TermMapper.cs
```
# Viktige deler

## XML-parser
XML-parseren leser XML-filen fra Mobility Online og gjør hver <row> om til et C#-objekt. Deretter hentes relevante felt ut og legges i en intern studentmodell.

Relevante filer:
```text
MobilityOnlineXmlReader.cs
MobilityOnlineRow.cs
MobilityOnlineParser.cs
MobilityOnlineStudent.cs
```
## DTO-er og payload
DTO-ene definerer datastrukturen som brukes internt i applikasjonen og strukturen som sendes videre til FS. FsPayloadModels.cs bestemmer hvordan JSON-payloaden til FS skal se ut.

Relevante filer:
```text
FsPayloadModels.cs
GraphQlRequest.cs
FsGraphQlOptions.cs
```

## Transformasjon til FS-format
Transformasjonslaget mapper MO-data til FS-format. Her bygges blant annet kullId, studentgrunnlagId, studierettstatusId, fraTermin og FS-vennlig fødselsnummer.

Relevante filer:
```text
FsTransformationService.cs
FoedselsnummerService.cs
TermMapper.cs
FsIntegrationOptions.cs
```
## Sending til FS
Data sendes til FS via GraphQL-mutasjonen opprettStudenter. Applikasjonen bygger en GraphQL-request, serialiserer den til JSON og sender den som en HTTP POST-request til FS sitt GraphQL-endepunkt.
Relevant fil:
```text
FsGraphQlClient.cs
```
## Konfigurasjon

Sensitive verdier som API-token skal ikke ligge i GitHub. Disse settes som miljøvariabler ved kjøring.
Brukte miljøvariabler:
```
FsGraphQl__Endpoint
FsGraphQl__ApiKey
FsGraphQl__AuthorizationHeader
```
Eksempel:
```bash

export FsGraphQl__Endpoint="https://api-test.fsweb.no/graphql"
export FsGraphQl__ApiKey="DIN_TOKEN"
export FsGraphQl__AuthorizationHeader="Basic

```

## Kjøre lokalt med .NET
Fra prosjektroten:
```bash
dotnet restore
dotnet build src/MoFs.Prototype/MoFs.Prototype.csproj
```

## Kjør applikasjonen med XML-fil:
```bash
dotnet run --project src/MoFs.Prototype/MoFs.Prototype.csproj -- "/path/til/test.xml"
```
Eksempel:
```bash
dotnet run --project src/MoFs.Prototype/MoFs.Prototype.csproj -- "/home/woraw/test.xml"
```

## Bygge container med Podman
Fra prosjektroten:

```bash
podman build -t localhost/mo-fs-prototype .
```

## Kjøre container med Podman

```bash
podman run --rm \
  -v /path/til/test.xml:/data/test.xml \
  -e FsGraphQl__Endpoint="https://api-test.fsweb.no/graphql" \
  -e FsGraphQl__ApiKey="DIN_TOKEN" \
  -e FsGraphQl__AuthorizationHeader="Basic" \
  localhost/mo-fs-prototype \
  /data/test.xml
```
## Output

Programmet skriver ut hvilke studenter som forsøkes sendt til FS, og viser responsen fra GraphQL-API-et.
Eksempel på vellykket respons:
```bash
{
  "data": {
    "opprettStudenter": {
      "errors": null
    }
  }
}
```
Eksempel på oppsummering etter prototypen er kjørt:
```bash
========================================
OPPSUMMERING
========================================

SENDT OK (1):
- Fornavn1 Etternavn1 | Fødselsnummer1

IKKE SENDT (1):
- Fornavn2 Etternavn2 | Fødselsnummer2 | FS svarte med feil
```
