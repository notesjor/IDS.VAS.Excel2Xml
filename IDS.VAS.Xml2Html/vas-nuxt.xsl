<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	exclude-result-prefixes="xs"
	version="2.0">

<xsl:output method="html"
			media-type="text/html" 
			encoding="UTF-8" 
			indent="no" />

<!-- 
	WICHTIG: sonst können bei Familienartikeln die zugehörigen Muster
             nicht aus der Index-Datei gelesen werden.
             Kann auch in OXYGEN im Transformationsszenario ${cfn} eingestellt
             werden
-->
<xsl:param name="FILENAME">''</xsl:param>

<!-- Literaturverzeichnis -->
<xsl:variable name="literatur" select="document('../vas/literatur.xml')" />


<!-- einmal hier, damit getNumber das nicht jedesmal wieder zusammensuchen muss -->
<xsl:variable name="SAMPLES_COLLECTION" select="//examples/xref except 
	                                            //predicate-list//examples/xref" />

<!--
<xsl:variable name="SAMPLES_COLLECTION" select="//overview//examples/xref |
	                                            //meaning//examples/xref |
	                                            //forms//examples/xref |
	                                            //predicates/section/examples/xref"  /> -->


<xsl:variable name="ARTIKEL_CLASS">
	<xsl:value-of select="normalize-space(/vas-artikel/head/meta[@type='class'])"/>
</xsl:variable>
	
<xsl:variable name="INDEX_PATH">
	<xsl:value-of select="lower-case(normalize-space(/vas-artikel/head/meta[@type='prep']))"/>
</xsl:variable>
	
<xsl:variable name="ARTIKEL_INDEX" select="document(concat('../artikel/', $INDEX_PATH, '/_index.xml'))" />
<!--
	<xsl:variable name="ARTIKEL_INDEX" select="document('../artikel/vor/_index.xml')" />
-->

<xsl:template match="/">
	
	<!-- diese Fehler sollten eigentlich schon während Bearbeitung behoben worden sein -->	
	<xsl:if test="not($ARTIKEL_CLASS eq 'Überblicksartikel' or
					  $ARTIKEL_CLASS eq 'Familienartikel' or
					  $ARTIKEL_CLASS eq 'Musterartikel')">
		<xsl:message>ERROR: Die Artikelklasse (Überblicksartikel, Familienartikel oder Musterartikel) 
			muss angegeben werden. Aktueller Wert: '<xsl:value-of select="$ARTIKEL_CLASS"/>'</xsl:message>
	</xsl:if>
	
	<xsl:if test="not(string-length($INDEX_PATH) gt 0)">
		<xsl:message>ERROR: Das Feld im Head [meta type="prep"] muss gefüllt werden; z.B. mit 'vor' (ohne Anführungszeichen)</xsl:message>
	</xsl:if>
	

<div class="test-wrapper">
	<xsl:apply-templates select="/vas-artikel/body"/>
</div>

</xsl:template>
	
	
<!-- 
	wird nich mehr benötigt ... löschen
<xsl:template name="header-info">
	<div class="header-info">		
		<table>
			<tr>
				<td>Präposition</td>
				<td><xsl:value-of select="/vas-artikel/head/meta[@type='prep']"/></td>
			</tr>
			<tr>
				<td>Familie</td>
				<td><xsl:value-of select="/vas-artikel/head/meta[@type='family']"/></td>
			</tr>
			<tr>
				<td>Typ</td>
				<td><xsl:value-of select="/vas-artikel/head/meta[@type='type']"/></td>
			</tr>
			<tr>
				<td>Tags</td>
				<td><xsl:value-of select="/vas-artikel/head/meta[@type='keyword']"/></td>
			</tr>
			<tr>
				<td>Autor*in</td>
				<td><xsl:value-of select="/vas-artikel/head/meta[@type='author']"/></td>
			</tr>
		</table>
	</div>
</xsl:template>
	-->
	
<xsl:template match="body">
	
	<xsl:call-template name="BuildPageNavigation" />
	
	<div class="vas-doc">
		
		<div class="head-lzga">
			<div class="lzga"><xsl:value-of select="/vas-artikel/head/meta[@type='name']"/></div>
			<div class="typ-ang"><xsl:value-of select="/vas-artikel/head/meta[@type='type']"/></div>
		</div>
		
		<xsl:apply-templates select="overview"/>
		
		<xsl:if test="$ARTIKEL_CLASS eq 'Familienartikel'">
			<xsl:call-template name="buildChildrenTOC" />
		</xsl:if>
		
		<xsl:apply-templates select="meaning"/>
		
		<xsl:apply-templates select="section"/>
		
		<xsl:apply-templates select="forms"/>
		<xsl:apply-templates select="predicates" />
		<xsl:apply-templates select="references" />
	</div>
</xsl:template>
	
<xsl:template name="BuildPageNavigation">
	<nav class="pagenav">
		<ul class="pagenav__toc">
			<xsl:apply-templates select="overview | meaning |
				.//section | forms | predicates | references" mode="pagenav"/>
		</ul>
	</nav>
</xsl:template>
	
<xsl:template match="overview" mode="pagenav">
	<li><a href="#overview">Überblick</a></li>
</xsl:template>
	
<xsl:template match="meaning" mode="pagenav">
	<li><a href="#meaning">Bedeutung</a></li>
</xsl:template>

<xsl:template match="section" mode="pagenav">
	<xsl:if test="@label">
		<li class="section"><a href="#{@label}"><xsl:value-of select="@label" /></a></li>
	</xsl:if>
</xsl:template>

<xsl:template match="forms" mode="pagenav">
	<li>
		<div><a href="#forms">Form</a></div>
		<ul class="pagenav__forms">
			<xsl:apply-templates select=".//pattern" mode="pagenav" />
		</ul>
	</li>
</xsl:template>
	
<xsl:template match="pattern" mode="pagenav">
	<li>
		<a href="#{generate-id()}">
			<div class="pattern-small">
				
				<xsl:for-each select="pitem">
					<xsl:call-template name="getPITEM-VALUE2" />
				</xsl:for-each>
				
			</div>
		</a>
	</li>
</xsl:template>
<!--
	<xsl:template match="pattern" mode="pagenav">
		<li>
			<a href="#{generate-id()}">
				<table>
					<tr>
						<xsl:for-each select="pitem">
							<xsl:call-template name="getPITEM-VALUE" />
						</xsl:for-each>
					</tr>
				</table>
			</a>
		</li>
	</xsl:template>	
-->	

<xsl:template match="predicates" mode="pagenav">
	<li><a href="#predicates">Prädikate</a></li>
</xsl:template>
	
<xsl:template match="references" mode="pagenav">
	<li><a href="#references">Literatur</a></li>
</xsl:template>
	
	
	
	
<xsl:template match="overview">
<section>
	<h1 id="overview">Überblick</h1>
	
	<xsl:choose>
		<xsl:when test="$ARTIKEL_CLASS eq 'Musterartikel'">
			<xsl:apply-templates select="prototype" mode="musterartikel" />
		</xsl:when>
		<xsl:otherwise>
			<xsl:apply-templates select="prototype"  />
		</xsl:otherwise>
	</xsl:choose>	
</section>
</xsl:template>
	
	
<xsl:template match="meaning">
	<section>
		<h1 id="meaning">Bedeutung</h1>
		<xsl:apply-templates />
	</section>
</xsl:template>

<!-- 
	forms (form-grp)+
	form-grp ((akt | med | pass)*, section*)
		@label ... optional
-->
<xsl:template match="forms">
	<section>
		<h1>Form</h1>
		<xsl:apply-templates />
	</section>
</xsl:template>
	
<xsl:template match="form-grp">
	<section>
		<xsl:if test="@label">
			<h2><xsl:value-of select="@label"/></h2>
		</xsl:if>
		
		<xsl:if test="akt">
			<div class="forms-abs">
				<div class="forms-abs__label">
					<span>Aktivisch</span>
				</div>
				<div>
					<xsl:apply-templates select="akt" />
				</div>
			</div>
		</xsl:if>
		
		<xsl:if test="kon">
			<div class="forms-abs">
				<div class="forms-abs__label">
					<span>Konvers</span>
				</div>
				<div>
					<xsl:apply-templates select="kon" />
				</div>
			</div>
		</xsl:if>
		
		<xsl:if test="pass">
			<div class="forms-abs">
				<div class="forms-abs__label">
					<span>Passivisch</span>
				</div>
				<div>
					<xsl:apply-templates select="pass" />
				</div>
			</div>
		</xsl:if>
		
		<xsl:if test="ambig">
			<div class="forms-abs">
				<div class="forms-abs__label">
					<span>Ambig</span>
				</div>
				<div>
					<xsl:apply-templates select="ambig" />
				</div>
			</div>
		</xsl:if>
		
		<xsl:apply-templates select="section" />
		
	</section>
</xsl:template>

<xsl:template match="akt | med | pass">
	<xsl:variable name="cn" select="local-name()"/>
	<div class="form-grp {$cn}">
		<xsl:apply-templates />	
	</div>
</xsl:template>


	
<xsl:template match="prototype">
	<div class="prototype"><xsl:apply-templates /></div>
</xsl:template>	
	
<xsl:template match="prototype" mode="musterartikel">
	
	<div class="prototype">
		<div class="overview-p">
			<xsl:apply-templates select="./p" />
		</div>
		
		<h4>Prototypische Realisierung:</h4>
		<div class="overview-pattern">
			<xsl:apply-templates select="./pattern" />
			
			<!-- erste Example-Block ohne Überschrift mir hierher -->
			<xsl:apply-templates select="./examples[1]" />
		</div>
		
		<h4>Weitere Beispiele:</h4>
		<xsl:apply-templates select="./examples[2]" />
	</div>
	
</xsl:template>

<xsl:template match="pattern">
	<div class="pattern-large" id="{generate-id()}">
		<xsl:for-each select="pitem">
			<xsl:call-template name="createPitemBlock" />
		</xsl:for-each>
	</div>
</xsl:template>
	


<!--
	<xsl:template match="pattern">
		<table class="pattern-table" id="{generate-id()}">
			<tr>
				<th>SEM</th>
				<xsl:for-each select="pitem">
					<xsl:call-template name="getPITEM-TableHead" />
				</xsl:for-each>
			</tr>
			<tr>
				<td>SYN</td>
				<xsl:for-each select="pitem">
					<xsl:call-template name="getPITEM-TableContent" />
				</xsl:for-each>
			</tr>
		</table>
	</xsl:template>	
-->
	
<xsl:template name="createPitemBlock">
	<xsl:choose>
		<xsl:when test="@slot = 'rel'">
			<div class="pitem-column">
				<div class="head rel"><xsl:value-of select="./@sem"/></div>
				<div class="pitem rel">
					<xsl:value-of select="./@syn"/>
					<!-- pitem sibling @slot=ktype einbauen -->
					<xsl:call-template name="getKTYPE" />
				</div>
			</div>
		</xsl:when>
		<xsl:when test="@slot = 'figure'">
			<div class="pitem-column">
				<div class="head figure">FIGUR</div>
				<div class="pitem figure"><xsl:value-of select="./@syn"/></div>
			</div>
		</xsl:when>
		<xsl:when test="@slot = 'ground'">
			<div class="pitem-column">
				<div class="head ground">GRUND</div>
				<div class="pitem ground">
					<xsl:call-template name="formatGround">
						<xsl:with-param name="g-value" select="./@syn" />
					</xsl:call-template>
				</div>
			</div>
		</xsl:when>
		<xsl:when test="@slot = 'effector'">
			<div class="pitem-column">
				<div class="head effector">AUSLÖSER</div>
				<div class="pitem effector"><xsl:value-of select="./@syn" /></div>
			</div>
		</xsl:when>
	</xsl:choose>
</xsl:template>	
	
<!-- CONTEXT ist pitem sibling -->	
<xsl:template name="getKTYPE">
	<xsl:variable name="k_type" select="../pitem[@slot='ktype']/@syn"/>
	
	<xsl:if test="$k_type">
		<span class="ktype"><xsl:value-of select="$k_type" /></span>
	</xsl:if>
	
</xsl:template>
	
<!-- context node ist ein pitem element -->
<xsl:template name="getPITEM-TableHead">
	<xsl:choose>
		<xsl:when test="@slot = 'rel'">
			<th class="rel"><xsl:value-of select="./@sem"/></th>
		</xsl:when>
		<xsl:when test="@slot = 'figure'">
			<th class="figure">FIGUR</th>
		</xsl:when>
		<xsl:when test="@slot = 'ground'">
			<th class="ground">GRUND</th>
		</xsl:when>
		<xsl:when test="@slot = 'effector'">
			<th class="effector">AUSLÖSER</th>
		</xsl:when>
	</xsl:choose>
</xsl:template>
	
<xsl:template name="getPITEM-TableContent">
	<xsl:choose>
		<xsl:when test="@slot = 'rel'">
			<td class="pitem rel"><xsl:value-of select="./@syn"/></td>
		</xsl:when>
		<xsl:when test="@slot = 'figure'">
			<td class="pitem figure"><xsl:value-of select="./@syn"/></td>
		</xsl:when>
		<xsl:when test="@slot = 'ground'">
			<td class="pitem ground">
				<xsl:call-template name="formatGround">
					<xsl:with-param name="g-value" select="./@syn" />
				</xsl:call-template>
			</td>
		</xsl:when>
		<xsl:when test="@slot = 'effector'">
			<td class="pitem effector"><xsl:value-of select="./@syn" /></td>
		</xsl:when>
	</xsl:choose>
</xsl:template>
	
<xsl:template name="getPITEM-VALUE2">
	<xsl:choose>
		<xsl:when test="@slot = 'rel'">
			<div class="pitem rel"><xsl:value-of select="./@syn"/></div>
		</xsl:when>
		<xsl:when test="@slot = 'figure'">
			<div class="pitem figure"><xsl:value-of select="./@syn"/></div>
		</xsl:when>
		<xsl:when test="@slot = 'ground'">
			<div class="pitem ground">
				<xsl:call-template name="formatGround">
					<xsl:with-param name="g-value" select="./@syn" />
				</xsl:call-template>
			</div>
		</xsl:when>
		<xsl:when test="@slot = 'effector'">
			<div class="pitem effector"><xsl:value-of select="./@syn" /></div>
		</xsl:when>
	</xsl:choose>
</xsl:template>
	
<!-- 
	INDEX_PATH ist die Präposition	
-->	
<xsl:template name="formatGround">
	<xsl:param name="g-value" />
	<span><xsl:value-of select="upper-case($INDEX_PATH)"/><sub> + <xsl:value-of select="$g-value"/></sub></span>
</xsl:template>

	
<!-- 
	Liste von xref	
-->
<xsl:template match="examples">
	<div class="examples">
		<xsl:apply-templates select="xref" mode="examples" />
	</div>
</xsl:template>

<!-- 
	Mode löst Referenz auf Samples auf	
-->
<xsl:template match="xref" mode="examples">
	<xsl:variable name="id" select="@href"/>
	<xsl:choose>
		<xsl:when test="/vas-artikel/body/samples/sample[@id eq $id]">
			<xsl:apply-templates select="/vas-artikel/body/samples/sample[@id eq $id]" />
		</xsl:when>
		<xsl:otherwise>
			<div class="sample error">Kein Beispiel mit @id 
				[<xsl:value-of select="$id"/>] gefunden.
			</div>
		</xsl:otherwise>
	</xsl:choose>
	
</xsl:template>
	
<xsl:template match="xref">
	<xsl:variable name="id" select="@href"/>
	
	<xsl:variable name="no">
		<!-- <xsl:value-of select="substring(@id,3)"/> -->
		<xsl:call-template name="getSampleNumber">
			<xsl:with-param name="id"><xsl:value-of select="$id"/></xsl:with-param>
		</xsl:call-template>
	</xsl:variable>
	
	<span class="xref-lnk"><xsl:value-of select="$no"/></span>
</xsl:template>
	
<xsl:template match="sample">
	<xsl:variable name="no">
		<!-- <xsl:value-of select="substring(@id,3)"/> -->
		<xsl:call-template name="getSampleNumber">
			<xsl:with-param name="id"><xsl:value-of select="@id"/></xsl:with-param>
		</xsl:call-template>
	</xsl:variable>
	
	<div class="sample">
		<div class="sample-nr">(<xsl:value-of select="$no"/>)</div>
		<div class="sample-txt"><xsl:apply-templates /><xsl:text> </xsl:text>
			<xsl:if test="@cosmas">
				<span class="cosmas-id">(<xsl:value-of select="@cosmas" />)</span>
			</xsl:if>
		</div>
	</div>
</xsl:template>	


<xsl:template match="section">
	<xsl:variable name="label" select="@label"/>
	
	<div class="sub-sec {$label}">
		<xsl:if test="$label">
			<h2 id="{$label}">
				<xsl:call-template name="getLabelText">
					<xsl:with-param name="label" select="$label" />
				</xsl:call-template>
			</h2>
		</xsl:if>
		<xsl:apply-templates />
	</div>
</xsl:template>

<xsl:template match="p">
	<p><xsl:apply-templates /></p>
</xsl:template>




<!-- 
	sections als Zwischenelemente zur Gliederung/ggf. Überschriften	
-->
<xsl:template match="predicates">
	<section>
		<h1 id="predicates">Prädikate</h1>
		<xsl:apply-templates />
	</section>
</xsl:template>
	
<xsl:template match="predicate-list">

	<div class="pred-list">
		
		<span class="pred-list__label">
			<xsl:call-template name="getLabelText">
				<xsl:with-param name="label" select="@label" />
			</xsl:call-template>: </span>
		
		<span class="pred-list__body">
			<xsl:for-each select="./predicate">
				<xsl:value-of select="@value" />
				<xsl:choose>
					<xsl:when test="position() = last()">
						<xsl:text>.</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>, </xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
		</span>
	</div>
	
	<!-- nur zur Korrektur -->
	<div class="pred-list--expanded">
		<xsl:for-each select="./predicate">
			<div><xsl:value-of select="@value" />:</div>
			<ul>
				<li><xsl:apply-templates /></li>
			</ul>
			
		</xsl:for-each>
		
	</div>
	
	
</xsl:template>



<xsl:template match="references">
<section>
	<h1 id="references">Literatur</h1>
	<div>
		<ul class="bibl-list">
			<xsl:apply-templates mode="references-block" />
		</ul>
	</div>
</section>
</xsl:template>
	
	<xsl:template match="reference" mode="references-block">
	<li>
		<xsl:apply-templates />
	</li>
</xsl:template>
	
<!-- inline -->
<xsl:template match="bibl-ref">
	<xsl:variable name="check-val">
		<xsl:call-template name="existsExternReferenceEntry">
			<xsl:with-param name="sigle">
				<xsl:value-of select="./@href"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:variable>
	
	<span class="bibl-ref {$check-val}"><xsl:value-of select="./@href" /></span>
</xsl:template>
	
<xsl:template match="bibl-ref" mode="references-block">
	<xsl:variable name="check-val">
		<xsl:call-template name="existsExternReferenceEntry">
			<xsl:with-param name="sigle">
				<xsl:value-of select="./@href"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:variable>
	
	<xsl:variable name="sigle">
		<xsl:value-of select="./@href"/>
	</xsl:variable>
	
	<li class="extern {$check-val}">
		<xsl:choose>
			<xsl:when test="$check-val eq 'error'">
				Sigle '<xsl:value-of select="./@href" />' nicht gefunden.
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="$literatur/references/reference[@id eq $sigle]" />
			</xsl:otherwise>
		</xsl:choose>
	</li>
</xsl:template>
	
	
<!-- INLINE -->
<xsl:template match="rel | effector | figure | ground">
	<xsl:variable name="class-name" select="local-name()" />
	
	<xsl:variable name="TRANSLATE">
		<xsl:if test="./text() eq 'Grund' or 
			          ./text() eq 'Figur' or
			          ./text() eq 'Auslöser'">
			<xsl:text>allcaps</xsl:text>
		</xsl:if>
	</xsl:variable>
	
	<span class="{$class-name} {$TRANSLATE}"><xsl:apply-templates /></span>
</xsl:template>

<xsl:template match="val">
	<span class="val"><xsl:apply-templates /></span>
</xsl:template>

<xsl:template match="b">
	<b><xsl:apply-templates /></b>
</xsl:template>

<xsl:template match="i">
	<i><xsl:apply-templates /></i>
</xsl:template>


<xsl:template match="sup">
	<sup><xsl:apply-templates /></sup>
</xsl:template>

<xsl:template match="sub">
	<sub><xsl:apply-templates /></sub>
</xsl:template>

<xsl:template match="obj-spr">
	<em><xsl:apply-templates /></em>
</xsl:template>
	
<xsl:template match="hi">
	<span class="hi {@class}"><xsl:apply-templates /></span>
</xsl:template>

<xsl:template match="link">
	<a href="#"><xsl:apply-templates /></a>
</xsl:template>
	

<!-- 
	braucht $FILENAME und die $ARTIKEL_INDEX	
-->
<xsl:template name="buildChildrenTOC">
<section class="family">
	<h1>Zugehörige Muster</h1>
	<ul class="family-list">
		<xsl:apply-templates select="$ARTIKEL_INDEX//family[@id eq $FILENAME]" mode="index"/>
	</ul>
</section>	
</xsl:template>
	
<!-- Elemente aus _index.xml -->
<xsl:template match="family" mode="index">
	
	<xsl:for-each select="child::*">
		<li><xsl:value-of select="@label" /></li>
	</xsl:for-each>
	
</xsl:template>

	
	
	
	
<!-- FUNCTIONS -->
<xsl:template name="getSampleNumber">
	<xsl:param name="id" select="''" />
	
	<!-- kann mehrere Treffer haben, wenn XREF mehrmals vorkommt -->
	<xsl:variable name="number-str">
		<xsl:for-each select="$SAMPLES_COLLECTION">
			<xsl:if test="@href eq $id">
				<xsl:text>;</xsl:text><xsl:value-of select="position()"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>
	
	<xsl:value-of select="substring($number-str,2)"/>
	
</xsl:template>

<!-- anders als in Autor-xsl findet hier keine Prüfung statt -->
<xsl:template name="getLabelText">
	<xsl:param name="label" select="''" />
	<xsl:value-of select="$label" />
</xsl:template>
	
	
<xsl:template name="existsExternReferenceEntry">
	<xsl:param name="sigle" select="''" />
	
	<xsl:choose>
		<xsl:when test="$literatur/references/reference[@id eq $sigle]">ok</xsl:when>
		<xsl:otherwise>error</xsl:otherwise>
	</xsl:choose>
</xsl:template>
	
</xsl:stylesheet>
	
	
	



