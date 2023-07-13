namespace ExcelParser

open System
open System.IO
open DocumentFormat.OpenXml.Packaging
open DocumentFormat.OpenXml.Spreadsheet
open Common
open InnaNor.API.Models
open Microsoft.EntityFrameworkCore
open Microsoft.FSharp.Core

module SpaceData =

    type BanedataHeader =
        | Objekt
        | Lokasjon
        | Delstrekning
        | Beskrivelse
        | Navn
        | Fra
        | Til
        | Serienr
        | Sportype
        | Spornummer
        | Side_fra
        | Side_til
        | Avst_spormidt
        | Status
        | Sist_endret_av
        | Sist_endret_dato
        | Tilhører_objekt
        | Baneprioritet
        | Idriftssatt_dato
        | Eier
        | Ekst_eier
        | Vernekategori
        | Lokasjonstatus
        | Type_spor
        | Hensettingslengde_m
        | Lengde_m
        | Fra_signal
        | Til_signal
        | KL_anlegg
        | Nummer_på_skillebryter
        | Servicerampe
        | Lengde_servicerampe_m
        | Avising_glykol
        | Vannpåfylling
        | Togvaskemaskin
        | Dieselpåfylling
        | Merknader
        | Helsveist_Lasket
        | Servicekiosk_VVS_Strøm
        | Servicehus_for_renholds_leverandør
        | Graffitifjerning
        | Sportilgang_med_bil_Kioskvarer_og_vedlikehold
        | Toalettømming_med_bil
        | Toalettømming_stasjonært
        | Banenummer
        | Banesjef
        | Område
        | Bane
        | Start_Nord
        | Start_Øst
        | Slutt_Nord
        | Slutt_Øst
        | UTM_sone
        | Merknad_langbeskrivelse
        | Antall_vedlegg

    let banedataHeader =
        function
        | "Objekt" -> Objekt
        | "Lokasjon" -> Lokasjon
        | "Delstrekning" -> Delstrekning
        | "Beskrivelse" -> Beskrivelse
        | "Navn/Nr" -> Navn
        | "Fra" -> Fra
        | "Til" -> Til
        | "Serienr" -> Serienr
        | "Sportype" -> Sportype
        | "Spornummer" -> Spornummer
        | "Side fra" -> Side_fra
        | "Side til" -> Side_til
        | "Avst. spormidt" -> Avst_spormidt
        | "Status" -> Status
        | "Sist endret av" -> Sist_endret_av
        | "Sist endret dato" -> Sist_endret_dato
        | "Tilhører objekt" -> Tilhører_objekt
        | "Baneprioritet" -> Baneprioritet
        | "Idriftssatt dato" -> Idriftssatt_dato
        | "Eier" -> Eier
        | "Ekst. eier" -> Ekst_eier
        | "Vernekategori" -> Vernekategori
        | "Lokasjonstatus" -> Lokasjonstatus
        | "Type spor" -> Type_spor
        | "Hensettingslengde (m)" -> Hensettingslengde_m
        | "Lengde (m)" -> Lengde_m
        | "Fra signal" -> Fra_signal
        | "Til signal" -> Til_signal
        | "KL-anlegg" -> KL_anlegg
        | "Nummer på skillebryter" -> Nummer_på_skillebryter
        | "Servicerampe" -> Servicerampe
        | "Lengde servicerampe (m)" -> Lengde_servicerampe_m
        | "Avising/glykol" -> Avising_glykol
        | "Vannpåfylling" -> Vannpåfylling
        | "Togvaskemaskin" -> Togvaskemaskin
        | "Dieselpåfylling" -> Dieselpåfylling
        | "Merknader" -> Merknader
        | "Helsveist/Lasket" -> Helsveist_Lasket
        | "Servicekiosk VVS/Strøm" -> Servicekiosk_VVS_Strøm
        | "Servicehus for renholds-leverandør" -> Servicehus_for_renholds_leverandør
        | "Graffitifjerning" -> Graffitifjerning
        | "Sportilgang med bil. Kioskvarer og vedlikehold" -> Sportilgang_med_bil_Kioskvarer_og_vedlikehold
        | "Toalettømming med bil" -> Toalettømming_med_bil
        | "Toalettømming stasjonært" -> Toalettømming_stasjonært
        | "Banenummer" -> Banenummer
        | "Banesjef" -> Banesjef
        | "Område" -> Område
        | "Bane" -> Bane
        | "Start Nord" -> Start_Nord
        | "Start Øst" -> Start_Øst
        | "Slutt Nord" -> Slutt_Nord
        | "Slutt Øst" -> Slutt_Øst
        | "UTM-sone" -> UTM_sone
        | "Merknad (langbeskrivelse)" -> Merknad_langbeskrivelse
        | "Antall vedlegg" -> Antall_vedlegg
        | header -> failwith $"headeren {header} er ikke i listen over gyldige headere"

    let banedataColumnStringValue (rawValues: Map<BanedataHeader, Cell>) (banedataHeader: BanedataHeader) =
        rawValues.TryFind banedataHeader

    let getCells (row: Row) : Cell seq = row.Elements<Cell>()

    exception private TableNotFoundException

    let rec private findTable getCellValue (rows: Row list) =
        match rows with
        | row :: remainingRows ->
            let cells = getCells row

            if Seq.length cells > 4 then
                let header =
                    cells |> Seq.map getCellValue |> Seq.map banedataHeader |> List.ofSeq

                (header, remainingRows)
            else
                findTable getCellValue remainingRows

        | [] -> raise TableNotFoundException

    let private cellByColumn (header: BanedataHeader list) (cell: Cell) =
        let cellReference = cell.CellReference.Value
        let _, cellHorizontalIndex = parseCellReference cellReference
        (header[cellHorizontalIndex], cell)

    let rec private collectSpaces
        getCellStringValue
        (collectedSpaces: Space list)
        (locations: Map<string, Location>)
        (rawSpaces: Map<BanedataHeader, Cell> list)
        =
        match rawSpaces with
        | rawSpace :: remainingRawSpaces ->

            let cellStringOption =
                banedataColumnStringValue rawSpace >> (Option.map getCellStringValue)

            let cellStringValue = cellStringOption >> (Option.defaultValue "")

            let cellIntValue = cellStringValue >> int
            let cellDecimalValue = cellStringValue >> decimal

            let cellNullableDecimalValue =
                cellStringOption >> Option.map decimal >> Option.toNullable

            let cellDateTimeValue = cellStringOption >> Option.get >> parseStrangeExcelDate

            let cellDateValue: BanedataHeader -> Nullable<DateOnly> =
                cellStringOption
                >> Option.map (parseStrangeExcelDate >> DateOnly.FromDateTime)
                >> Option.toNullable

            let locationId = cellStringValue Lokasjon

            let location =
                match locations.TryFind locationId with
                | Some location -> location
                | None ->
                    Location(
                        Id = locationId,
                        Status = (cellStringValue Status),
                        TrackNumber = (cellStringValue Banenummer),
                        TrackResponsible = (cellStringValue Banesjef),
                        Area = (cellStringValue Område),
                        Track = (cellStringValue Bane)
                    )
                    
            let globalSpaceId = cellStringValue Objekt
            let localSpaceId = globalSpaceId.Split("-")[2] |> int
            let space =
                Space(
                    Id = localSpaceId,
                    GlobalId = globalSpaceId,
                    LocationId = locationId,
                    Location = location,
                    Beskrivelse = cellStringValue Beskrivelse,
                    Name = cellStringValue Navn,
                    From = cellDecimalValue Fra,
                    To = cellDecimalValue Til,
                    TrackType = cellStringValue Sportype,
                    TrackId = cellStringValue Spornummer,
                    Status = cellStringValue Status,
                    LastChangedBy = cellStringValue Sist_endret_av,
                    LastChanged = cellDateTimeValue Sist_endret_dato,
                    BelongsTo = cellStringValue Tilhører_objekt,
                    TrackPriority = cellIntValue Baneprioritet,
                    ActiveFrom = cellDateValue Idriftssatt_dato,
                    Owner = cellStringValue Eier,
                    TrackUsageType = cellStringValue Type_spor,
                    TrainPlacementLength = cellNullableDecimalValue Hensettingslengde_m,
                    Length = cellNullableDecimalValue Lengde_m,
                    FromSignal = cellStringValue Fra_signal,
                    ToSignal = cellStringValue Til_signal,
                    // KlAnlegg = cellValue KlAnlegg ,
                    // NummerPåSkillebryter = cellValue NummerPåSkillebryter ,
                    ServiceRamp = cellStringValue Servicerampe,
                    DeIcing = cellStringValue Avising_glykol,
                    WaterFilling = cellStringValue Vannpåfylling,
                    TrainWashing = cellStringValue Togvaskemaskin,
                    DieselRefueling = cellStringValue Dieselpåfylling,
                    Notes = cellStringValue Merknader,
                    // HelsveistLasket = cellValue HelsveistLasket ,
                    // ServicekioskVvsStrøm = cellValue ServicekioskVvsStrøm ,
                    ServiceHouseForCleaningSuppliers = cellStringValue Servicehus_for_renholds_leverandør,
                    GraffitiRemoval = cellStringValue Graffitifjerning,
                    AccessibleByCarKioskResupplyAndService =
                        cellStringValue Sportilgang_med_bil_Kioskvarer_og_vedlikehold,
                    SewageEmptyingByCar = cellStringValue Toalettømming_med_bil,
                    SewageEmptyingStationary = cellStringValue Toalettømming_stasjonært,
                    StartNorth = cellStringValue Start_Nord,
                    StartEast = cellStringValue Start_Øst,
                    EndNorth = cellStringValue Slutt_Nord,
                    EndEast = cellStringValue Slutt_Øst
                )

            collectSpaces
                getCellStringValue
                (space :: collectedSpaces)
                (locations.Add(locationId, location))
                remainingRawSpaces
        | [] -> (locations, collectedSpaces)

    let private spacesFactory
        getCellStringValue
        (locations: Map<string, Location>)
        (rawSpaces: Map<BanedataHeader, Cell> list)
        =
        collectSpaces getCellStringValue List.empty locations rawSpaces


    let private cellValuesByColumn header row =
        let cells = getCells row
        let cellsByColumn = cells |> Seq.map (cellByColumn header)
        cellsByColumn |> Map.ofSeq

    let rec private createSpaces (sharedStrings: Map<string, string>) rows =
        let getCellStringValue = getCellStringValue sharedStrings

        let header, dataRows =
            try
                findTable getCellStringValue rows
            with TableNotFoundException ->
                ([], [])

        let cellsByColumn = cellValuesByColumn header

        let cells = dataRows |> List.map cellsByColumn
        spacesFactory getCellStringValue Map.empty cells


    let parse (path: string) =
        use doc = SpreadsheetDocument.Open(path, false)
        let wbPart = doc.WorkbookPart

        let sharedStringTablePart =
            wbPart.GetPartsOfType<SharedStringTablePart>() |> Seq.head

        let sharedStrings =
            sharedStringTablePart.SharedStringTable.ChildElements
            |> Seq.mapi (fun i e -> string i, e.InnerText)
            |> Map.ofSeq

        let spacesFromRows = createSpaces sharedStrings

        let sheets =
            wbPart.Workbook.Descendants<Sheet>()
            |> Seq.map (fun sheet -> sheet.Id, sheet.Name.Value)

        for sheetId, sheetName in sheets do
            printfn $"%s{sheetName}"
            let wsPart = wbPart.GetPartById(sheetId) :?> WorksheetPart
            let rows = wsPart.Worksheet.Descendants<Row>() |> List.ofSeq

            let locations, spaces = spacesFromRows rows

            use dbContext =
                new InnaNorContext((DbContextOptionsBuilder<InnaNorContext>().UseSqlite "Data Source=../../../../innanor.db").Options)
                
            dbContext.Locations.AddRange locations.Values
            dbContext.Spaces.AddRange spaces
            
            dbContext.SaveChanges() |> ignore
