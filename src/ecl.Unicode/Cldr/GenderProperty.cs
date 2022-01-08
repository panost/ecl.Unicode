namespace ecl.Unicode.Cldr {

    /// <summary>
    /// Classifies nouns in classes.
    /// </summary>
    /// <adapted>
    /// linguistics-ontology.org/gold/2010/GenderProperty
    /// </adapted>
    /// <remarks>
    /// https://unicode-org.github.io/cldr/ldml/tr35-general.html#Gender
    /// </remarks>
    public enum GenderProperty {

        /// <summary>
        /// In an animate/inanimate gender system, gender that denotes human or animate entities.
        /// </summary>
        /// <remarks>
        /// description adapted from: wikipedia.org/wiki/Grammatical_gender, linguistics-ontology.org/gold/2010/AnimateGender
        /// </remarks>
        Animate,

        /// <summary>
        /// In an animate/inanimate gender system, gender that denotes object or inanimate entities .
        /// </summary>
        /// <remarks>
        /// adapted from: wikipedia.org/wiki/Grammatical_gender, linguistics-ontology.org/gold/2010/InanimateGender
        /// </remarks>
        Inanimate,

        /// <summary>
        /// In an animate/inanimate gender system in some languages, gender that specifies the masculine gender of animate entities.
        /// </summary>
        /// <remarks>
        /// adapted from: wikipedia.org/wiki/Grammatical_gender, linguistics-ontology.org/gold/2010/HumanGender
        /// </remarks>
        Personal,

        /// <summary>
        /// In a common/neuter gender system, gender that denotes human entities.
        /// </summary>
        /// <remarks>
        /// adapted from: wikipedia.org/wiki/Grammatical_gender
        /// </remarks>
        Common,

        /// <summary>
        /// In a masculine/feminine or in a masculine/feminine/neuter gender system, gender that denotes specifically female persons (or animals) or that is assigned arbitrarily to object.
        /// </summary>
        /// <remarks>
        /// adapted from: http://wikipedia.org/wiki/Grammatical_gender, linguistics-ontology.org/gold/2010/FeminineGender
        /// </remarks>
        Feminine,

        /// <summary>
        /// In a masculine/feminine or in a masculine/feminine/neuter gender system, gender that denotes specifically male persons (or animals) or that is assigned arbitrarily to object.
        /// </summary>
        /// <remarks>
        /// adapted from: wikipedia.org/wiki/Grammatical_gender, linguistics-ontology.org/gold/2010/MasculineGender
        /// </remarks>
        Masculine,

        /// <summary>
        /// In a masculine/feminine/neuter or common/neuter gender system, gender that generally denotes an object.
        /// </summary>
        /// <remarks>
        /// adapted from: wikipedia.org/wiki/Grammatical_gender, linguistics-ontology.org/gold/2010/NeuterGender
        /// </remarks>
        Neuter
    }
}