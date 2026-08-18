using NUnit.Framework;
using FeeSystem;
using System.Collections.Generic;

namespace FeeSystem.Tests;

[TestFixture]
public class FeeCalculatorTests
{
    private FeeCalculator _calculator;

    [SetUp]
    public void Setup()
    {
        _calculator = new FeeCalculator();
    }

    // 1. No payments → full fee outstanding
    [Test]
    public void OutstandingBalance_NoPayments_ReturnsFullFee()
    {
        var payments = new List<decimal>();
        var result = _calculator.OutstandingBalance(600m, payments);
        Assert.That(result, Is.EqualTo(600m));
    }

    // 2. One partial payment (600 fee, 200 paid → 400)
    [Test]
    public void OutstandingBalance_OnePartialPayment_ReturnsCorrectBalance()
    {
        var payments = new List<decimal> { 200m };
        var result = _calculator.OutstandingBalance(600m, payments);
        Assert.That(result, Is.EqualTo(400m));
    }

    // 3. Several instalments (200 + 200 + 100 → 100)
    [Test]
    public void OutstandingBalance_MultiplePayments_ReturnsCorrectBalance()
    {
        var payments = new List<decimal> { 200m, 200m, 100m };
        var result = _calculator.OutstandingBalance(600m, payments);
        Assert.That(result, Is.EqualTo(100m));
    }

    // 4. Fee fully paid → balance 0
    [Test]
    public void OutstandingBalance_FullyPaid_ReturnsZero()
    {
        var payments = new List<decimal> { 600m };
        var result = _calculator.OutstandingBalance(600m, payments);
        Assert.That(result, Is.EqualTo(0m));
    }

    // 5. Overpayment (600 fee, 700 paid → -100)
    [Test]
    public void OutstandingBalance_Overpayment_ReturnsNegative()
    {
        var payments = new List<decimal> { 700m };
        var result = _calculator.OutstandingBalance(600m, payments);
        Assert.That(result, Is.EqualTo(-100m));
    }

    // 6. Negative fee → throws ArgumentException
    [Test]
    public void OutstandingBalance_NegativeFee_ThrowsArgumentException()
    {
        var payments = new List<decimal>();
        Assert.That(() => _calculator.OutstandingBalance(-1m, payments), 
            Throws.ArgumentException);
    }

    // 7. Exactly half paid → cleared for exams is true
    [Test]
    public void IsClearedForExams_ExactlyHalfPaid_ReturnsTrue()
    {
        var payments = new List<decimal> { 300m }; 
        var result = _calculator.IsClearedForExams(600m, payments);
        Assert.That(result, Is.True);
    }

    // 8. One toea under half → cleared is false
    [Test]
    public void IsClearedForExams_JustUnderHalfPaid_ReturnsFalse()
    {
        var payments = new List<decimal> { 299.99m }; 
        var result = _calculator.IsClearedForExams(600m, payments);
        Assert.That(result, Is.False);
    }
}   
