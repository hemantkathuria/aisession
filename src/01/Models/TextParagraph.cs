using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


namespace _01.Models
{
    internal class TextParagraph
    {
        /// <summary>A unique key for the text paragraph.</summary>
        [VectorStoreRecordKey]
        [TextSearchResultName]
        public required string Key { get; init; }

        /// <summary>A uri that points at the original location of the document containing the text.</summary>
        [VectorStoreRecordData]
        [TextSearchResultLink]
        public required string DocumentUri { get; init; }

        /// <summary>The id of the paragraph from the document containing the text.</summary>
        [VectorStoreRecordData]
        public required string ParagraphId { get; init; }

        /// <summary>The text of the paragraph.</summary>
        [VectorStoreRecordData]
        [TextSearchResultValue]
        public required string Text { get; init; }

        /// <summary>The embedding generated from the Text.</summary>
        /// This is the dimension size of the vector and has to match the size of vector that your chosen embedding generator produces.
        [VectorStoreRecordVector(1536)]
        public ReadOnlyMemory<float> TextEmbedding { get; set; }
    }
}
