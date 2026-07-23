using IPService.Application.Commands.IpInfo;
using IPService.Application.Interfaces;
using IPService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace IPService.Tests
{
    public class RefreshAllIpsHandlerTests
    {
        [Fact]
        public async Task Handle_ProcessesBatchAndSavesHistory()//1 batch -> processes, counts, saves history
        {
            //arrange
            var ips = new List<Ipv4Address>
            {
                new Ipv4Address(10, "US", DateTime.UtcNow),
                new Ipv4Address(20, "GR", DateTime.UtcNow),
            };

            //batch: 1st call -> IPs, 2nd call -> empty 
            var fakeBatch = new Mock<IGetIpBatchPersistence>();
            fakeBatch
                .SetupSequence(x => x.GetBatchAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ips)//1st: batch with 2 IPs
                .ReturnsAsync(new List<Ipv4Address>());//2nd: empty -> break

            //every update return true
            var fakeMediator = new Mock<IMediator>();
            fakeMediator
                .Setup(x => x.Send(It.IsAny<UpdateIpInfoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var fakeLogger = new Mock<ILogger<RefreshAllIpsHandler>>();
            var fakeSaveHistory = new Mock<ISaveJobHistory>();

            var handler = new RefreshAllIpsHandler(
                fakeBatch.Object, fakeMediator.Object, fakeLogger.Object, fakeSaveHistory.Object);

            //act - big batch size so it will be short page -> if (batch.Count < batchSize) break;
            var result = await handler.Handle(new RefreshAllIpsCommand(BatchSize: 100), default);

            //assert
            Assert.Equal(2, result.Processed);//2 ips edited
            Assert.Equal(2, result.Changed);//2 changed mediator -> true
            Assert.Equal(0, result.Failed);//no failed

            
            //update called 2 times?
            fakeMediator.Verify(
                x => x.Send(It.IsAny<UpdateIpInfoCommand>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            //jobhistory saved 1 time?
            fakeSaveHistory.Verify(
                x => x.SaveAsync(It.IsAny<JobHistory>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}