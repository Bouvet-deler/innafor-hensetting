namespace ExcelParser

open System
open DocumentFormat.OpenXml.Spreadsheet

module Common =

    let private firstRowCharacterIndex = (int '0')

    let private parseRowIndex oldIndex character =
        oldIndex * 10 + ((int character) - firstRowCharacterIndex)

    let private firstColumnCharacterIndex = (int 'A') - 1
    let private lastColumnCharacterIndex = (int 'Z')
    let private columnDigit = lastColumnCharacterIndex - firstColumnCharacterIndex

    let private parseColumnIndex oldIndex character =
        oldIndex * columnDigit + ((int character) - firstColumnCharacterIndex)

    let parseCellReference (cellReference: string) =
        let cellReferenceCharacters = cellReference |> Seq.toList

        let rec calculate characters rowIndex columnIndex =
            match characters with
            | r :: remainingCharacters when r >= '0' && r <= '9' ->
                calculate remainingCharacters (parseRowIndex rowIndex r) columnIndex
            | c :: remainingCharacters when c >= 'A' && c <= 'Z' ->
                calculate remainingCharacters rowIndex (parseColumnIndex columnIndex c)
            | [] -> rowIndex - 1, columnIndex - 1
            | chars -> raise (invalidArg (nameof cellReference) $"the given cellReference '{cellReference}' contains one or more invalid characters '{chars[0]}'")

        calculate cellReferenceCharacters 0 0

    let getCellStringValue (sharedStrings: Map<string, string>) (cell: Cell) =
        if cell.DataType = CellValues.SharedString then
            sharedStrings[cell.InnerText]
        else
            cell.InnerText

    // Not 100% sure this will always return the correct date,
    // but it should be sufficient for our use
    let parseStrangeExcelDate (excelDateString: string) =
        excelDateString |> double |> DateTime.FromOADate
