namespace Innanor.ExcelParser.Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open ExcelParser

[<TestClass>]
type CommonTests () =

    [<TestMethod>]
    [<DataRow(0, 0, "A1")>]
    [<DataRow(0, 1, "B1")>]
    [<DataRow(0, 2, "C1")>]
    [<DataRow(0, 23, "X1")>]
    [<DataRow(0, 25, "Z1")>]
    [<DataRow(0, 26, "AA1")>]
    [<DataRow(0, 51, "AZ1")>]
    [<DataRow(0, 676, "ZA1")>]
    [<DataRow(10, 676, "ZA11")>]
    [<DataRow(1, 0, "A2")>]
    [<DataRow(9, 0, "A10")>]
    [<DataRow(10, 0, "A11")>]
    [<DataRow(1, 1, "B2")>]
    member this.ParseCellReferenceReturnsCorrectIndexes (expectedRowIndex: int, expectedColumnIndex: int, cellReference: string) =
        let actualRowIndex, actualColumnIndex = Common.parseCellReference cellReference
        Assert.AreEqual((expectedRowIndex, expectedColumnIndex), (actualRowIndex, actualColumnIndex))

    [<TestMethod>]
    [<ExpectedException (typeof<ArgumentException>)>]
    [<DataRow("a1")>]
    [<DataRow(";1")>]
    [<DataRow("ø1")>]
    [<DataRow("A+")>]
    member this.ParseCellReferenceFailsOnInvalidInput (cellReference: string) =
        Common.parseCellReference cellReference |> ignore
