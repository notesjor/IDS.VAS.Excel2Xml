<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	exclude-result-prefixes="xs"
	version="2.0">

<xsl:output method="html"
			media-type="text/html" 
			encoding="UTF-8" 
			indent="no" />


<xsl:param name="PRINT"        select="'nein'" />

<!-- 
	WICHTIG: sonst können bei Familienartikeln die zugehörigen Muster
             nicht aus der Index-Datei gelesen werden.
             Kann auch in OXYGEN im Transformationsszenario ${cfn} eingestellt
             werden
    ANM. 22.02.22:
    	Die Indexdatei wird z.Z. nicht benötigt. Dafür muss aber auf die zu einer
    	Familie zugehörigen Muster zugegriffen werden können; oder auch auf Muster
    	anderer Familien/Präpositionen?
    	Deswegen den ARTIKEL_ROOT_PFAD, von dem aus LINKS aufgelöst werden können.
-->
<xsl:param name="FILENAME">''</xsl:param>
<xsl:param name="DIR">''</xsl:param>

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
	
<xsl:variable name="PRAEPOSITION">
	<xsl:value-of select="lower-case(normalize-space(/vas-artikel/head/meta[@type='prep']))"/>
</xsl:variable>
	
<!--
	<xsl:variable name="ARTIKEL_INDEX" select="document(concat('../artikel/', $PRAEPOSITION, '/_index.xml'))" />
	<xsl:variable name="ARTIKEL_INDEX" select="document('../artikel/vor/_index.xml')" />
-->

<xsl:template match="/">
	
	<!-- diese Fehler sollten eigentlich schon während Bearbeitung behoben worden sein -->	
	<xsl:if test="not($ARTIKEL_CLASS eq 'Markerartikel' or
					  $ARTIKEL_CLASS eq 'Familienartikel' or
					  $ARTIKEL_CLASS eq 'Musterartikel')">
		<xsl:message>ERROR: Die Artikelklasse (Überblicksartikel, Familienartikel oder Musterartikel) 
			muss angegeben werden. Aktueller Wert: '<xsl:value-of select="$ARTIKEL_CLASS"/>'</xsl:message>
	</xsl:if>
	
	<xsl:if test="not(string-length($PRAEPOSITION) gt 0)">
		<xsl:message>ERROR: Das Feld im Head [meta type="prep"] muss gefüllt werden; z.B. mit 'vor' (ohne Anführungszeichen)</xsl:message>
	</xsl:if>

		<div class="test-wrapper">			
			<xsl:apply-templates select="/vas-artikel/body"/>
		</div>

</xsl:template>
	

<xsl:template match="body">
	
	<xsl:if test="($ARTIKEL_CLASS eq 'Musterartikel') and ($PRINT eq 'nein')">
		<xsl:call-template name="BuildPageNavigation" />
	</xsl:if>
	
	<div class="vas-doc">
		
		<div class="head-lzga">
			<div class="lzga"><xsl:value-of select="/vas-artikel/head/meta[@type='name']"/></div>
			<div class="typ-ang"><xsl:value-of select="/vas-artikel/head/meta[@type='type']"/></div>
		</div>
		
		<xsl:apply-templates select="overview"/>
		<xsl:apply-templates select="meaning"/>
		<xsl:apply-templates select="section" mode="Level01"/>
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

<!-- 
	So ein Label wie 'Besonderheiten' kann mehrfach vorkommen (war anders vereinbart,
    aber es kommt wie es kommt)
    
    
-->
<xsl:template match="section" mode="pagenav">
	<!-- leere label Attribute ... test auf existenz nicht hinreichend ... seufz -->
	<xsl:if test="string-length(@label) gt 0">
		<xsl:variable name="n_id">
			<xsl:value-of select="../local-name()"/>/<xsl:value-of select="@label"/>
		</xsl:variable>
		
		<li class="section"><a href="#{$n_id}"><xsl:value-of select="@label" /></a></li>
	</xsl:if>
</xsl:template>

<xsl:template match="forms" mode="pagenav">
	<li>
		<div><a href="#forms">Form</a></div>
		<ul class="pagenav__forms">
			<xsl:for-each select="form-grp">
				<xsl:apply-templates mode="pagenav" />
			</xsl:for-each>
		</ul>
	</li>
</xsl:template>
	
<xsl:template match="akt | kon | pass | ambig" mode="pagenav">
	<xsl:variable name="var_dia">
		<xsl:call-template name="getDiatheseLabel">
			<xsl:with-param name="tagname" select="local-name()"></xsl:with-param>
		</xsl:call-template>
	</xsl:variable>
	
	<xsl:variable name="var_casus">
		<xsl:call-template name="getFormGrpLabel">
			<xsl:with-param name="label" select="../@label"></xsl:with-param>
		</xsl:call-template>
	</xsl:variable>
	
	<li class="pagenav__label">
		<span><xsl:value-of select="$var_dia"/></span>
		<span><xsl:value-of select="$var_casus"/></span>
	</li>
	
	<xsl:apply-templates mode="pagenav" />
</xsl:template>
	
<xsl:template match="pattern" mode="pagenav">
	<li>
		<a href="#{@id}">
			<div class="pattern-small">
				<xsl:for-each select="pitem">
					<xsl:call-template name="getPITEM-VALUE2" />
				</xsl:for-each>
			</div>
		</a>
	</li>
</xsl:template>
	

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
		<h1 id="forms">Form</h1>
		<xsl:apply-templates />
	</section>
</xsl:template>
	
<xsl:template match="form-grp">
	
	<xsl:variable name="block_label">
		<xsl:choose>
			<xsl:when test="@label">
				<xsl:call-template name="getFormGrpLabel">
					<xsl:with-param name="label" select="@label" />
				</xsl:call-template>
			</xsl:when>
			<!-- Leerer String -->
			<xsl:otherwise></xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	
	<section class="forms-abs">
		<xsl:if test="$block_label">
			<div class="forms-abs__label">
				<span><xsl:value-of select="$block_label"/></span>
			</div>
		</xsl:if>
		
		<div>
			<xsl:if test="akt">
				<div class="forms-abs">
					<div class="forms-abs__label">
						<span>aktivisch</span>
					</div>
					<div>
						<xsl:apply-templates select="akt" />
					</div>
				</div>
			</xsl:if>
			
			<xsl:if test="kon">
				<div class="forms-abs">
					<div class="forms-abs__label">
						<span>konvers</span>
					</div>
					<div>
						<xsl:apply-templates select="kon" />
					</div>
				</div>
			</xsl:if>
			
			<xsl:if test="pass">
				<div class="forms-abs">
					<div class="forms-abs__label">
						<span>passivisch</span>
					</div>
					<div>
						<xsl:apply-templates select="pass" />
					</div>
				</div>
			</xsl:if>
			
			<xsl:if test="ambig">
				<div class="forms-abs">
					<div class="forms-abs__label">
						<span>ambig</span>
					</div>
					<div>
						<xsl:apply-templates select="ambig" />
					</div>
				</div>
			</xsl:if>
			
			<xsl:apply-templates select="section" />
		</div>
	</section>
</xsl:template>

<xsl:template match="akt | kon | pass | ambig">
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
	<div class="pattern-large" id="{@id}">
		<xsl:for-each select="pitem">
			<xsl:call-template name="createPitemBlock" />
		</xsl:for-each>
	</div>
</xsl:template>
	
	
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
	PRAEPOSITION ist die Präposition	
-->	
<xsl:template name="formatGround">
	<xsl:param name="g-value" />
	<span><xsl:value-of select="upper-case($PRAEPOSITION)"/><sub> + <xsl:value-of select="$g-value"/></sub></span>
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
	Eigentlich das gleiche, nur sollen keine automatischen Zahlen
	vor den Samples angezeigt werden
-->
<xsl:template match="examples" mode="pred-list">
	<div class="examples">
		<xsl:apply-templates select="xref" mode="pred-list" />
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
	
<!-- 
	Im Prinzip wie Standard-Mode "examples"; 
	Nur wird ein Samples Template aufgerufen, dass 
		* keine Nummerierung generiert
		* keine ID
-->	
<xsl:template match="xref" mode="pred-list">
	<xsl:variable name="id" select="@href"/>
	<xsl:choose>
		<xsl:when test="/vas-artikel/body/samples/sample[@id eq $id]">
			<xsl:apply-templates 
				select="/vas-artikel/body/samples/sample[@id eq $id]" 
				mode="pred-list"/>
		</xsl:when>
		<xsl:otherwise>
			<div class="sample error">Kein Beispiel mit @id 
				[<xsl:value-of select="$id"/>] gefunden.
			</div>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>
	
<!-- 
	Querverweis im TEXT	
-->
<xsl:template match="xref">
	<xsl:variable name="id" select="@href"/>
	
	<xsl:variable name="no">
		<!-- <xsl:value-of select="substring(@id,3)"/> -->
		<xsl:call-template name="getSampleNumber">
			<xsl:with-param name="id"><xsl:value-of select="$id"/></xsl:with-param>
		</xsl:call-template>
	</xsl:variable>
	
	<a class="xref-lnk" href="#{@href}"><xsl:value-of select="$no"/></a>
</xsl:template>
	
<xsl:template match="sample">
	<xsl:variable name="no">
		<!-- <xsl:value-of select="substring(@id,3)"/> -->
		<xsl:call-template name="getSampleNumber">
			<xsl:with-param name="id"><xsl:value-of select="@id"/></xsl:with-param>
		</xsl:call-template>
	</xsl:variable>
	
	<div class="sample" id="{@id}">
		<div class="sample-nr">(<xsl:value-of select="$no"/>)</div>
		
		<xsl:if test="@etc">
			<div class="sample-pref">
				<xsl:value-of select="@etc"/>
			</div>
		</xsl:if>
		
		<div class="sample-txt">
			<xsl:apply-templates /><xsl:text> </xsl:text>
			<xsl:if test="@cosmas">
				<span class="cosmas-id">(<xsl:value-of select="@cosmas" />)</span>
			</xsl:if>
		</div>
	</div>
</xsl:template>
	
<!-- 
	@etc wird hier ignoriert. Hier sollte es auch keine konstruierten
    Belege geben.	
-->
<xsl:template match="sample" mode="pred-list">
	<div id="{@id}" class="sample-txt">
		<xsl:apply-templates /><xsl:text> </xsl:text>
		<xsl:if test="@cosmas">
			<span class="cosmas-id">(<xsl:value-of select="@cosmas" />)</span>
		</xsl:if>
	</div>
</xsl:template>

<!-- 
	Section Kind von
		meaning, forms, predicates	
	unterscheiden von 
		body > section
		
	wird in Body mit mode aufgerufen ... ist einfacher
-->
<xsl:template match="section" mode="Level01">
	<xsl:variable name="sec_id">
		<xsl:choose>
			<xsl:when test="@label">
				<xsl:value-of select="@label"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>sec_</xsl:text><xsl:value-of select="position()"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	
	<section id="{$sec_id}">
		<xsl:if test="@label">
			<h1><xsl:value-of select="@label"/></h1>
		</xsl:if>
		<xsl:apply-templates />
	</section>
</xsl:template>	
	
<xsl:template match="section">
	<xsl:variable name="label" select="@label"/>
	<xsl:variable name="class" select="@class"/>
	
	<div class="sub-sec {$class}">
		<xsl:if test="$label">
			
			<xsl:variable name="n_id">
				<xsl:value-of select="../local-name()"/>/<xsl:value-of select="@label"/>
			</xsl:variable>
			
			<h2 id="{$n_id}">
				<xsl:call-template name="getLabelText">
					<xsl:with-param name="label" select="$label" />
				</xsl:call-template>
			</h2>
		</xsl:if>
		<xsl:apply-templates />
	</div>
</xsl:template>
	
	
<!-- 
	ELEMENT illustration (img, caption?) 
	ELEMENT caption (#PCDATA) 
	ELEMENT img EMPTY 
		@src CDATA #REQUIRED 
-->	

<xsl:template match="illustration">
	<div class="illustration"><xsl:apply-templates /></div>
</xsl:template>
	
<xsl:template match="img">
	<img src="{@src}" />
</xsl:template>
	
<xsl:template match="caption">
	<div><xsl:apply-templates /></div>
</xsl:template>
	

<xsl:template match="p">
	<p><xsl:apply-templates /></p>
</xsl:template>

<xsl:template match="ul">
<ul>
	<xsl:choose>
		<xsl:when  test="@class eq 'pattern-list'">
			<xsl:apply-templates mode="pattern-list" />
		</xsl:when>
		<xsl:otherwise>
			<xsl:apply-templates></xsl:apply-templates>
		</xsl:otherwise>
	</xsl:choose>
</ul>
</xsl:template>

<!-- normale LI -->
<xsl:template match="li">
	<li><xsl:apply-templates /></li>
</xsl:template>

<!-- holen sich das erste sample vom referenzierten Muster -->
<xsl:template match="li" mode="pattern-list">
	
	<!-- 
		Links in der Form: PRÄP / MUSTER_NAME
	-->
	<xsl:variable name="pattern-name">
		<xsl:value-of select="substring-after(link/@href, '/')" />
	</xsl:variable>
	
	<!-- PFADE sind RELATIV zum XSLT-STYLESHEET !!! -->
	<!-- <xsl:variable name="PATTERN_ARTIKEL" select="document('../artikel/beispiele jan-22/auftritt.xml')" /> -->
		
	<xsl:variable name="PATTERN_ARTIKEL" select="document(concat($DIR, $pattern-name, '.xml'))" />
	
	

	<xsl:variable name="example">
		<xsl:value-of select="$pattern-name" />
	</xsl:variable>
	
	<li>
		<div class="pattern-list__body"><xsl:apply-templates /></div>
		<div class="pattern-list__sample">
			<xsl:apply-templates 
				select="$PATTERN_ARTIKEL//overview/prototype/examples[1]/xref" 
				mode="pred-list" />
		</div>
	</li>
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
	
	<xsl:variable name="n_id">
		<xsl:text>predlist_</xsl:text><xsl:value-of select="position()"/>
	</xsl:variable>

	<div class="pred-list">
		<input type="checkbox" id="{$n_id}" />
		
		<div class="pred-list__head">
			<div>
				<span class="pred-list__lblhead">
					<xsl:call-template name="getLabelText">
						<xsl:with-param name="label" select="@label" />
					</xsl:call-template>: </span>
				
				<span class="pred-list__lblbody">
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
			
			<div>
				<label for="{$n_id}">
					<span>Beispiele</span> 
					<span class="arrow"><svg><use href="#arrow"></use></svg></span>
				</label>
			</div>
		</div>
			
		<div class="pred-list__content">
			<xsl:if test="./predicate/@evalbu-ref">
				<xsl:call-template name="listEVALBU-Refs" />
			</xsl:if>
			
			<div>
				<xsl:for-each select="./predicate">
					<div class="predicate"><xsl:value-of select="@value" />:</div>
					<ul>
						<!-- 
							xref 
							habe von examples, xref, sample je mode:pred-list
							angelegt: Q&D
						-->
						<li><xsl:apply-templates mode="pred-list" /></li> 
					</ul>
				</xsl:for-each>
			</div>
		</div>
	</div>
</xsl:template>
	
<!-- 
	Context-Node ist eine pred-list	
-->	
<xsl:template name="listEVALBU-Refs">
	<div>
		<span class="pred-list__lblhead">Auch im 
			<a class="ext-lnk" href="https://grammis.ids-mannheim.de/verbvalenz">E-VALBU</a> behandelt: </span>
		<xsl:for-each select="./predicate[@evalbu-ref]">
			<xsl:variable name="ref" select="@evalbu-ref"/>
			<a class="ext-lnk" href="https://grammis.ids-mannheim.de/verbs/view/{$ref}"><xsl:value-of select="@value" /></a>
			<xsl:choose>
				<xsl:when test="position() = last()">
					<xsl:text>.</xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>, </xsl:text>
				</xsl:otherwise>
			</xsl:choose>
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
	<li id="{@id}">
		<xsl:apply-templates />
	</li>
</xsl:template>
	

<xsl:template match="bibl-ref">
	<xsl:variable name="sigle" select="@href"/>
	<xsl:variable name="pos" select="."/>
	<xsl:variable name="stellAng">
		<xsl:if test="string-length($pos) gt 0">
			<xsl:text>: </xsl:text>
			<xsl:value-of select="$pos"/>
		</xsl:if>
	</xsl:variable>
	
	<a class="bibl-ref" href="#{$sigle}"><xsl:value-of select="$sigle" /><xsl:value-of select="$stellAng" /></a>
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
	<xsl:template match="rel | val | vrb | prp | effector | figure | ground">
	<xsl:variable name="class-name" select="local-name()" />
	
	<xsl:variable name="TRANSLATE">
		<xsl:if test="count(text()) eq 1">
			<xsl:if test="./text() eq 'Grund' or 
				          ./text() eq 'Figur' or
				          ./text() eq 'Auslöser'">
				<xsl:text>allcaps</xsl:text>
			</xsl:if>
			
			<!-- 
				nope: solche HACKS machen wir seit 18.3. (s. DTD) nicht mehr.
				      gibt jetzt extra SLOT Elemente dafür
			-->
			<!--
			<xsl:if test="lower-case(./text()) eq $PRAEPOSITION or
					      @class eq 'hi'">
				<xsl:text>invers</xsl:text>
			</xsl:if>
			-->
			
		</xsl:if>
	</xsl:variable>
	
	<span class="{$class-name} {$TRANSLATE}"><xsl:apply-templates /></span>
</xsl:template>

<!-- 
	ist jetzt ein SLOT-ELEMENT	
-->
<!--
<xsl:template match="val">
	<span class="val"><xsl:apply-templates /></span>
</xsl:template>
-->

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
	
<xsl:template match="sc">
	<span class="sm-caps"><xsl:apply-templates /></span>
</xsl:template>

<xsl:template match="obj-spr">
	<em><xsl:apply-templates /></em>
</xsl:template>
	
<xsl:template match="hi">
	<span class="hi {@class}"><xsl:apply-templates /></span>
</xsl:template>

<!-- 
	s. DTD für verschiedene Typen

	TODO: Pfade müssen ggf. an Anwendung angepasst werden
-->
<xsl:template match="link">
	<xsl:variable name="lnk_typ">
		<xsl:choose>
			<xsl:when test="starts-with(@href, 'http')">ext-lnk</xsl:when>
			<xsl:when test="starts-with(@href, '/')">doc-lnk</xsl:when>
			<xsl:when test="starts-with(@href, '#')">i-lnk</xsl:when>
			<xsl:otherwise>entry-lnk</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	
	<a class="{$lnk_typ}" href="{@href}"><xsl:apply-templates /></a>
</xsl:template>
	

<!-- 
	braucht $FILENAME und die $ARTIKEL_INDEX
	
	Sehr schade. In der neuen Version von Familienartikeln
	wird diese Übersicht nicht mehr automatisch generiert,
	sondern ist ein geschreibener Text mit Verweisen.
-->
<!--
<xsl:template name="buildChildrenTOC">
<section class="family">
	<h1>Zugehörige Muster</h1>
	<ul class="family-list">
		<xsl:apply-templates select="$ARTIKEL_INDEX//family[@id eq $FILENAME]" mode="index"/>
	</ul>
</section>	
</xsl:template>
-->

<!-- Elemente aus _index.xml -->
<!--
<xsl:template match="family" mode="index">
	
	<xsl:for-each select="child::*">
		<li><xsl:value-of select="@label" /></li>
	</xsl:for-each>
	
</xsl:template>
-->
	
	
	
	
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
	
	
<xsl:template name="getFormGrpLabel">
	<xsl:param name="label" select="''" />
	
	<xsl:choose>
		<xsl:when test="$label eq 'AKK'">Akkusativ</xsl:when>
		<xsl:when test="$label eq 'DAT'">Dativ</xsl:when>
		<!-- FEHLER -->
		<xsl:otherwise>*</xsl:otherwise>
	</xsl:choose>
</xsl:template>
	
<!--
	in Form Grupp refactorieren, da die gleichen Strings	
-->
<xsl:template name="getDiatheseLabel">
	<xsl:param name="tagname" select="''" />
	
	<xsl:choose>
		<xsl:when test="$tagname eq 'akt'">Aktivisch</xsl:when>
		<xsl:when test="$tagname eq 'kon'">Konvers</xsl:when>
		<xsl:when test="$tagname eq 'pass'">Passivisch</xsl:when>
		<xsl:when test="$tagname eq 'ambig'">Ambig</xsl:when>
		<!-- FEHLER -->
		<xsl:otherwise>*</xsl:otherwise>
	</xsl:choose>
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
	
<!-- 
	frech geklaut aus NeoSearch...
	weitere Icons können leicht hingefügt werden.
	Verwendung z.B.
		<span className="arrow"><svg><use href="#arrow"></use></svg></span>	
-->	
<xsl:template name="SvgIcons">
	<svg style="display: 'none'">
		<symbol id="arrow" viewBox="0 0 24 24">
			<path d="M8.12,9.29L12,13.17l3.88-3.88c0.39-0.39,1.02-0.39,1.41,0l0,0c0.39,0.39,0.39,1.02,0,1.41l-4.59,4.59
				c-0.39,0.39-1.02,0.39-1.41,0l-4.59-4.59c-0.39-0.39-0.39-1.02,0-1.41l0,0C7.09,8.91,7.73,8.9,8.12,9.29z"/>
		</symbol>
	</svg>
</xsl:template>
	
</xsl:stylesheet>
	
	
	



