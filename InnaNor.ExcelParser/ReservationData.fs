module InnaNor.ExcelParser.ReservationData

open System
open DocumentFormat.OpenXml.Packaging
open DocumentFormat.OpenXml.Spreadsheet
open ExcelParser
open InnaNor.API.Models
open Microsoft.EntityFrameworkCore

type ParsedCell =
    { Ref: string
      ColIndex: int
      RowIndex: int
      Content: string
      Color: string option }

let parseCell (fillColor: string option array) getCellStringValue (cell: Cell) =
    let reference = cell.CellReference.Value
    let refRowIndex, columnIndex = Common.parseCellReference reference
    let fillColor = fillColor[int cell.StyleIndex.Value]
    let content = getCellStringValue cell

    { Ref = reference
      ColIndex = columnIndex
      RowIndex = refRowIndex + 1
      Content = content
      Color = fillColor }

type CellGroup =
    { Row: int
      StartCol: int
      EndCol: int
      Content: string list
      Color: string }

let cellGroupsFolder (cellGroups: CellGroup list) (cell: ParsedCell) =
    match cellGroups, cell.Color with
    | cellGroup :: otherGroups, Some(color) when cellGroup.EndCol + 1 = cell.ColIndex && color = cellGroup.Color ->
        { cellGroup with
            EndCol = cell.ColIndex
            Content = cell.Content :: cellGroup.Content }
        :: otherGroups
    | otherGroups, Some(color) ->
        { Row = cell.RowIndex
          StartCol = cell.ColIndex
          EndCol = cell.ColIndex
          Content = [ cell.Content ]
          Color = color }
        :: otherGroups
    | _ -> cellGroups

let extractCellGroups (cells: ParsedCell seq) =
    cells |> Seq.fold cellGroupsFolder List.empty

let parseCellGroupsFromRow cellFills getCellStringValue (row: Row) =
    let cells =
        row.Descendants<Cell>() |> Seq.map (parseCell cellFills getCellStringValue)

    let cellGroups = extractCellGroups cells
    cellGroups |> Seq.rev

type ParsedReservation =
    { Row: int
      StartTime: DateTime
      EndTime: DateTime
      Labels: string array
      Color: string }

let colIndexToQuartersFromStart colIndex = (colIndex - 2) * 15 |> double

let addQuarters (sheetStartDate: DateTime) quarters = sheetStartDate.AddMinutes(quarters)

let repeatingStringsFolder (result: string list) (current: string) =
    match result with
    | previous :: _ when previous = current -> result
    | result -> current :: result

let createReservationFromCellGroup (sheetStartDate: DateTime) (cellGroup: CellGroup) =
    let dateTimeFromColIndex =
        colIndexToQuartersFromStart >> (addQuarters sheetStartDate)

    { Row = cellGroup.Row
      StartTime = dateTimeFromColIndex cellGroup.StartCol
      EndTime = dateTimeFromColIndex (cellGroup.EndCol + 1)
      Labels =
        cellGroup.Content
        |> Seq.filter (fun l -> l <> "")
        |> Seq.toList
        |> List.fold repeatingStringsFolder List.empty
        |> Array.ofList
      Color = cellGroup.Color }

let getReservationsFromSheet (sharedStrings: Map<string, string>) cellFills (wsPart: WorksheetPart) =
    let sheet = wsPart.Worksheet

    let cells =
        sheet.Descendants<Cell>()
        |> Seq.map (fun cell -> cell.CellReference.Value, cell)
        |> Map.ofSeq

    let getCellStringValue = Common.getCellStringValue sharedStrings

    let sheetStartDate = cells["G2"].InnerText |> double |> DateTime.FromOADate

    let rows = sheet.Descendants<Row>() |> Seq.skip 4

    let cellGroups =
        rows |> Seq.collect (parseCellGroupsFromRow cellFills getCellStringValue)

    let reservations =
        cellGroups |> Seq.map (createReservationFromCellGroup sheetStartDate)

    reservations

let subsequentReservationMerger (state: ParsedReservation list) (reservation: ParsedReservation) =
    match state with
    | prevReservation :: processedReservations when
        prevReservation.EndTime = reservation.StartTime
        && prevReservation.Color = reservation.Color
        ->
        let prevLabels = prevReservation.Labels
        let labels = reservation.Labels

        let newLabels =
            if
                not (Array.isEmpty prevLabels)
                && not (Array.isEmpty labels)
                && (Array.last prevLabels) = labels[0]
            then
                Array.append prevLabels (Array.skip 1 labels)
            else
                Array.append prevLabels labels

        { prevReservation with
            EndTime = reservation.EndTime
            Labels = newLabels }
        :: processedReservations
    | processedReservations -> reservation :: processedReservations

let getCellStyleFills (wbPart: WorkbookPart) =
    let stylesPart = wbPart.GetPartsOfType<WorkbookStylesPart>() |> Seq.head

    let fills =
        stylesPart.Stylesheet.Fills
        |> Seq.map (fun f -> (f :?> Fill).PatternFill)
        |> Seq.toArray

    stylesPart.Stylesheet.CellFormats.Elements<CellFormat>()
    |> Seq.map (fun e ->
        if e.ApplyFill <> null && e.ApplyFill.Value then
            Some fills[int e.FillId.Value]
        else
            None)
    |> Seq.toArray
    |> Array.map (fun fo ->
        match fo with
        | Some(f) when f.PatternType.InnerText = "solid" && f.ForegroundColor <> null ->
            if f.ForegroundColor.Rgb <> null then
                Some(f.ForegroundColor.Rgb.InnerText)
            else if f.ForegroundColor.Indexed <> null && (int f.ForegroundColor.Indexed.Value) <> 65 then
                Some($"i{f.ForegroundColor.Indexed.Value}")
            else if f.ForegroundColor.Theme <> null && (int f.ForegroundColor.Theme.Value) <> 0 then
                Some($"t{f.ForegroundColor.Theme.Value}")
            else
                None
        | _ -> None)


let mappingFromRowToRowIdentifiers =
    [| (5, "Spor 1")
       (6, "Spor 1")
       (7, "Spor 1")
       (8, "Spor 2")
       (9, "Spor 2")
       (10, "Spor 2")
       (11, "Spor 3")
       (12, "Spor 3")
       (13, "Spor 3")
       (14, "Spor 4")
       (15, "Spor 4")
       (16, "Spor 4")
       (17, "Spor 5")
       (18, "Spor 5")
       (19, "Spor 5")
       (20, "Spor 6")
       (21, "Spor 6")
       (22, "Spor 6")
       (23, "Spor 7")
       (24, "Spor 7")
       (25, "Spor 7")
       (26, "Spor 8")
       (27, "Spor 8")
       (28, "Spor 8")
       (29, "Spor 9")
       (30, "Spor 9")
       (31, "Spor 9")
       (32, "Spor 10")
       (33, "Spor 10")
       (34, "Spor 10")
       (35, "Spor 11")
       (36, "Spor 11")
       (37, "Spor 11")
       (38, "Spor 12")
       (39, "Spor 12")
       (40, "Spor 12")
       (41, "Spor 13")
       (42, "Spor 13")
       (43, "Spor 13")
       (44, "Spor 13")
       (45, "Spor 14")
       (46, "Spor 14")
       (47, "Spor 14")
       (48, "Spor 14")
       (49, "Spor 15")
       (50, "Spor 15")
       (51, "Spor 15")
       (52, "Spor 15")
       (53, "Spor 16")
       (54, "Spor 16")
       (55, "Spor 16")
       (56, "Spor 16")
       (57, "Spor 17")
       (58, "Spor 17")
       (59, "Spor 17")
       (60, "Spor 18")
       (61, "Spor 18")
       (62, "Spor 18")
       (63, "Spor 19")
       (64, "Spor 19")
       (65, "Spor 20")
       (66, "Spor 20")
       (69, "K2")
       (70, "K2")
       (71, "K2")
       (72, "K1")
       (73, "K1")
       (74, "K1")
       (75, "Kværner - Spor K6")
       (76, "Kværner - Spor K5")
       (77, "Kværner - Spor K5")
       (78, "Kværner - Spor K5")
       (79, "TH 0")
       (80, "TH 1")
       (81, "TH 1")
       (82, "TH 1")
       (83, "TH 2")
       (84, "TH 2")
       (85, "TH 2")
       (86, "TH 3")
       (87, "TH 3")
       (88, "TH 3")
       (89, "TH 3")
       (90, "TH 4")
       (91, "TH 4")
       (92, "TH 4")
       (93, "Spor 32")
       (94, "Spor 32")
       (95, "Spor 32")
       (96, "Spor 31")
       (97, "Spor 31")
       (98, "Spor 31")
       (99, "Spor 30")
       (100, "Spor 30")
       (101, "Spor 30")
       (102, "Spor 29")
       (103, "Spor 29")
       (104, "Spor 29")
       (105, "Spor 28")
       (106, "Spor 27")
       (107, "Spor 26")
       (108, "Spor 26")
       (109, "Spor 26")
       (110, "Spor 25")
       (111, "Spor 25")
       (112, "Spor 25")
       (113, "Spor 23")
       (114, "Spor 23")
       (115, "Spor 21")
       (116, "Spor 21")
       (117, "Spor 22")
       (118, "Spor 22")
       (119, "Spor 34")
       (120, "Spor 35")
       (121, "Spor 35")
       (122, "Spor 35")
       (123, "Spor 35")
        |]
    |> Map.ofArray

let reservationEntityFactory (spaces: Space array) (reservation: ParsedReservation) =
    // Some small groups have the label written outside in the excel sheet
    // and is therefore missing the label here. Just skipping them this time
    if Array.isEmpty reservation.Labels then
        [||]
    else
        let name = mappingFromRowToRowIdentifiers[reservation.Row]
        let matchingSpaces = spaces |> Array.filter (fun s -> s.Name = name)
        let mergedReservationLabels = reservation.Labels |> (String.concat "; ")

        matchingSpaces
        |> Array.map (fun space ->
            let reserver =
                if Array.length reservation.Labels > 1 then
                    reservation.Labels[1]
                else
                    reservation.Labels[0]

            Reservation(
                SpaceId = space.Id,
                Space = space,
                Reserver = reserver,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                Notes = mergedReservationLabels
            ))

let saveReservations (reservations: ParsedReservation seq) =
    use dbContext =
        new InnaNorContext(
            (DbContextOptionsBuilder<InnaNorContext>().UseSqlite "Data Source=../../../../innanor.db")
                .Options
        )

    let spacesList = dbContext.Spaces
    let spaces =
        spacesList
        |> Seq.filter (fun s -> s.LocationId = "0020-01003" || s.LocationId = "0040-05002")
        |> Seq.toArray

    let reservationEntities =
        reservations |> Seq.collect (reservationEntityFactory spaces)

    dbContext.Reservations.AddRange reservationEntities
    dbContext.SaveChanges() |> ignore


let parse (path: string) =
    use doc = SpreadsheetDocument.Open(path, false)
    let wbPart = doc.WorkbookPart

    let sharedStrings = Common.getSharedStrings wbPart
    let cellFills = getCellStyleFills wbPart

    let reservationsFromSheetCollector =
        getReservationsFromSheet sharedStrings cellFills

    let reservations =
        Common.getSheets wbPart
        |> Seq.collect reservationsFromSheetCollector
        |> Seq.groupBy (fun r -> r.Row)
        |> Seq.collect (fun (_, g) ->
            g
            |> Seq.fold subsequentReservationMerger List.empty<ParsedReservation>
            |> Seq.rev)

    saveReservations reservations
