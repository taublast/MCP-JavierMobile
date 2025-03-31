using Microsoft.Extensions.AI;
using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer.Tools.Android
{
    [McpServerToolType]
    public class AndroidScreenshotLlmTool
    {
        const string VerifyScreenshotPrompt = @"
 		You are a software tool designed to analyze mobile application screenshots. The user will provide you with two screenshots and then ask to compare them. Your job is to analyze the screenshots, and return a boolean indicating whether they are equal or not. You should return responses in JSON object format. More specific instructions:
 		- Use computer vision techniques including OCR to look for differences, even if they are small.
 		- Don't include any text or conversation.
 		- Do not include any explanations of your choices.
 		- Reply simply with the boolean in string format.";

        /// <summary>
        /// Compares two screenshots using the provided prompt and an interaction with the Large Language Model (LLM).
        /// </summary>
        /// <param name="thisServer">The MCP server instance used for communication with the LLM.</param>
        /// <param name="screenshot1">The byte array representing the first screenshot image.</param>
        /// <param name="screenshot2">The byte array representing the second screenshot image.</param>
        /// <param name="prompt">The textual prompt to guide the LLM comparison process.</param>
        /// <param name="maxTokens">The maximum number of tokens to generate in the response from the LLM.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests, ensuring graceful termination of the operation.</param>
        /// <returns>
        /// A boolean value indicating whether the screenshots are deemed identical (true) or different (false) based on the LLM response.
        /// </returns>
        [McpServerTool("android_compare_screenshot_llm")]
        [Description("Compares two screenshots using the provided prompt and an interaction with the Large Language Model (LLM)")]
        public static async Task<bool> CompareScreenshotsLLM(IMcpServer thisServer, byte[] screenshot1, byte[] screenshot2, string prompt, int maxTokens, CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInfo("Starting comparison of screenshots via LLM.");

                // Validate inputs and log them
                if (screenshot1 == null || screenshot2 == null)
                {
                    Logger.LogError("One or both screenshot byte arrays are null.");
                    return false;
                }

                Logger.LogInfo("Preparing chat messages for LLM interaction.");
                ChatMessage[] messages =      
                [
                new(ChatRole.System, VerifyScreenshotPrompt),
                new ChatMessage(ChatRole.User,
                    new List<AIContent>
                    {
                        new TextContent(prompt ?? "Compare the images"),
                        new DataContent(screenshot1, "image/png"),
                        new DataContent(screenshot2, "image/png")
            
                    })
                ];

                ChatOptions options = new()
                {
                    MaxOutputTokens = maxTokens,
                    Temperature = 0.7f,
                };

                var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);

                if(response is not null)
                {
                    Logger.LogInfo("Received response from LLM.");
                    var result = response.Text;
                    bool.TryParse(result, out bool boolResult);

                    return boolResult;
                }
                else
                {
                    Logger.LogWarning("LLM returned a null response.");
                }

                return false;
            }
            catch(Exception ex)
            {
                Logger.LogException("Error comparing screenshots", ex);
                Logger.LogError($"Error comparing screenshots: {ex.Message}");
                return false;
            }
        }
    }
}