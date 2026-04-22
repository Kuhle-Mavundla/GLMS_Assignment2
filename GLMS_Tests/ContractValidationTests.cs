using Xunit;
using GLMS_Assignment2.Models;
using GLMS_Assignment2.Models.Enums;
using GLMS_Assignment2.Services;

namespace GLMS_Tests
{
    /// <summary>
    /// Tests for workflow / business validation logic.
    /// Verifies that ServiceRequests are correctly blocked for Expired and On Hold contracts.
    /// </summary>
    public class ContractValidationTests
    {
        private readonly ContractValidationService _service;

        public ContractValidationTests()
        {
            _service = new ContractValidationService();
        }

        [Fact]
        public void CanCreateServiceRequest_ActiveContract_ReturnsValid()
        {
            var contract = new Contract { Status = ContractStatus.Active };
            var (isValid, error) = _service.CanCreateServiceRequest(contract);
            Assert.True(isValid);
            Assert.Empty(error);
        }

        [Fact]
        public void CanCreateServiceRequest_DraftContract_ReturnsValid()
        {
            var contract = new Contract { Status = ContractStatus.Draft };
            var (isValid, error) = _service.CanCreateServiceRequest(contract);
            Assert.True(isValid);
            Assert.Empty(error);
        }

        [Fact]
        public void CanCreateServiceRequest_ExpiredContract_ReturnsInvalid()
        {
            var contract = new Contract { Status = ContractStatus.Expired };
            var (isValid, error) = _service.CanCreateServiceRequest(contract);
            Assert.False(isValid);
            Assert.Contains("Expired", error);
        }

        [Fact]
        public void CanCreateServiceRequest_OnHoldContract_ReturnsInvalid()
        {
            var contract = new Contract { Status = ContractStatus.OnHold };
            var (isValid, error) = _service.CanCreateServiceRequest(contract);
            Assert.False(isValid);
            Assert.Contains("On Hold", error);
        }

        [Fact]
        public void CanCreateServiceRequest_NullContract_ReturnsInvalid()
        {
            var (isValid, error) = _service.CanCreateServiceRequest(null!);
            Assert.False(isValid);
            Assert.Contains("not found", error);
        }

        [Fact]
        public void IsValidDateRange_EndAfterStart_ReturnsTrue()
        {
            Assert.True(_service.IsValidDateRange(
                new DateTime(2026, 1, 1),
                new DateTime(2026, 12, 31)));
        }

        [Fact]
        public void IsValidDateRange_EndBeforeStart_ReturnsFalse()
        {
            Assert.False(_service.IsValidDateRange(
                new DateTime(2026, 12, 31),
                new DateTime(2026, 1, 1)));
        }

        [Fact]
        public void IsValidDateRange_SameDate_ReturnsFalse()
        {
            var date = new DateTime(2026, 6, 15);
            Assert.False(_service.IsValidDateRange(date, date));
        }
    }
}